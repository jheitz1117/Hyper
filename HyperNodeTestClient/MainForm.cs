using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.SystemCommands.Contracts;
using Hyper.NodeServices.UnitTesting.Contracts.CommandModules;

namespace HyperNodeTestClient
{
    public partial class MainForm : Form
    {
        private const string ClientAgentName = "HyperNodeTestClient";

        private bool _aliceIsProgressTracking;

        public MainForm()
        {
            InitializeComponent();

            tvwRealTimeTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
            tvwAliceTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
            tvwBobTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
        }

        #region Events

        private async void btnRefreshCommandList_Click(object sender, EventArgs e)
        {
            try
            {
                cboCommandNames.DataSource = null;

                var serializer = new NetDataContractResponseSerializer<GetNodeStatusResponse>();
                var msg = new HyperNodeMessageRequest(ClientAgentName)
                {
                    CommandName = SystemCommandName.GetNodeStatus
                };

                using (var client = new HyperNodeClient("Alice"))
                {
                    var response = await client.ProcessMessageAsync(msg);

                    // TODO: Recursively find the response we're actually interested in

                    if (!string.IsNullOrWhiteSpace(response.CommandResponseString))
                    {
                        if (((ICommandResponseSerializer)serializer).Deserialize(response.CommandResponseString) is GetNodeStatusResponse commandResponse)
                            cboCommandNames.DataSource = commandResponse.Commands.Select(c => c.CommandName).OrderBy(cn => cn).ToList();
                    }
                    else
                    {
                        MessageBox.Show(response.RespondingNodeName + " did not send back a command response string.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRunCommand_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear out our data source first
                ClearResponseData();

                // Create our message request
                var serializer = new NetDataContractRequestSerializer<LongRunningCommandTestRequest>();
                var commandRequestString = serializer.Serialize(
                    new LongRunningCommandTestRequest
                    {
                        TotalRunTime = TimeSpan.FromHours(1),
                        MinimumSleepInterval = TimeSpan.FromSeconds(1),
                        MaximumSleepInterval = TimeSpan.FromSeconds(5)
                    }
                );

                var msg = new HyperNodeMessageRequest(ClientAgentName)
                {
                    CommandName = cboCommandNames.Text,
                    CommandRequestString = commandRequestString,
                    ProcessOptionFlags = (chkReturnTaskTrace.Checked ? MessageProcessOptionFlags.ReturnTaskTrace : MessageProcessOptionFlags.None) |
                                         (chkRunConcurrently.Checked ? MessageProcessOptionFlags.RunConcurrently : MessageProcessOptionFlags.None) |
                                         (chkCacheProgressInfo.Checked ? MessageProcessOptionFlags.CacheTaskProgress : MessageProcessOptionFlags.None)
                };

                using (var client = new HyperNodeClient("Alice"))
                {
                    var response = client.ProcessMessage(msg);

                    PopulateResponseSummary(lstRealTimeResponse, response);
                    PopulateTaskTrace(tvwRealTimeTaskTrace, response);

                    if (response.NodeAction != HyperNodeActionType.Rejected && msg.CacheTaskProgress)
                        StartAliceProgressTracking(response.TaskId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void aliceProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var progressTimer = new Stopwatch();
            progressTimer.Start();

            var taskProgressInfo = new HyperNodeTaskProgressInfo();
            using (var alice = new HyperNodeClient("Alice"))
            {
                var request = (HyperNodeMessageRequest)e.Argument;
                ICommandResponseSerializer serializer = new NetDataContractResponseSerializer<GetCachedTaskProgressInfoResponse>();

                while (!taskProgressInfo.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
                {
                    var aliceResponse = alice.ProcessMessage(request);

                    var targetResponse = aliceResponse;
                    if (string.IsNullOrWhiteSpace(targetResponse?.CommandResponseString))
                        break;

                    var commandResponse = (GetCachedTaskProgressInfoResponse)serializer.Deserialize(targetResponse.CommandResponseString);
                    taskProgressInfo = commandResponse.TaskProgressInfo ?? new HyperNodeTaskProgressInfo();
                    if (!commandResponse.TaskProgressCacheEnabled)
                    {
                        taskProgressInfo.Activity.Add(
                            new HyperNodeActivityItem(ClientAgentName)
                            {
                                EventDescription = "Warning: Task progress cache is not enabled for HyperNode \'Alice\'."
                            }
                        );

                        // Make sure we exit the loop, since we're not going to get anything useful in this case.
                        taskProgressInfo.IsComplete = true;
                    }

                    ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(taskProgressInfo.ProgressPercentage), taskProgressInfo);

                    Task.Delay(500).Wait();
                }
            }

            progressTimer.Stop();

            e.Result = taskProgressInfo;
        }

        private void aliceProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var taskProgressInfo = ((HyperNodeTaskProgressInfo)e.UserState);

            lstAliceActivityItems.DataSource = GetActivityStrings(taskProgressInfo.Activity);
        }

        private void aliceProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Alice's Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var taskProgressInfo = (HyperNodeTaskProgressInfo)e.Result;

                lstAliceActivityItems.DataSource = GetActivityStrings(taskProgressInfo.Activity);
                PopulateResponseSummary(lstAliceResponseSummary, taskProgressInfo.Response);
                PopulateTaskTrace(tvwAliceTaskTrace, taskProgressInfo.Response);
            }

            _aliceIsProgressTracking = false;
        }

        private static void tvwTaskTrace_OnBeforeExpand(object sender, TreeViewCancelEventArgs treeViewCancelEventArgs)
        {
            var expandingNode = treeViewCancelEventArgs.Node;

            if (expandingNode.Tag is ConcurrentDictionary<string, HyperNodeMessageResponse> expandingResponse)
            {
                expandingNode.Nodes.Clear();
                expandingNode.Nodes.AddRange(
                    expandingResponse.SelectMany(
                        kvp => new[]
                        {
                            new TreeNode(
                                kvp.Value.RespondingNodeName,
                                GetActivityStrings(kvp.Value.TaskTrace).Select(s=>new TreeNode(s)).ToArray()
                            )
                            {
                                Tag = kvp.Value
                            }
                        }
                    ).ToArray()
                );
            }
        }

        private void btnAliceCancelCurrentTask_Click(object sender, EventArgs e)
        {
            var targetTaskId = txtAliceTaskId.Text;
            if (!string.IsNullOrWhiteSpace(targetTaskId))
            {
                // Create our message request
                var msg = new HyperNodeMessageRequest(ClientAgentName)
                {
                    CommandName = SystemCommandName.CancelTask,
                    CommandRequestString = targetTaskId
                };

                using (var client = new HyperNodeClient("Alice"))
                {
                    client.ProcessMessage(msg);
                }
            }
        }

        private void btnBobCancelCurrentTask_Click(object sender, EventArgs e)
        {
            var targetTaskId = txtBobTaskId.Text;
            if (!string.IsNullOrWhiteSpace(targetTaskId))
            {
                // Create our message request
                var msg = new HyperNodeMessageRequest(ClientAgentName)
                {
                    CommandName = SystemCommandName.CancelTask,
                    CommandRequestString = targetTaskId
                };

                using (var client = new HyperNodeClient("Alice"))
                {
                    client.ProcessMessage(msg);
                }
            }
        }

        #endregion Events

        #region Private Methods

        private static string[] GetActivityStrings(IEnumerable<HyperNodeActivityItem> activity)
        {
            return (
                from a in activity
                orderby a.EventDateTime
                select FormatActivityItem(a)
            ).ToArray();
        }

        private static string FormatActivityItem(HyperNodeActivityItem item)
        {
            return $"{item.EventDateTime:G} {item.Agent}{(item.ProgressPercentage.HasValue || item.Elapsed.HasValue ? $" ({item.Elapsed}{(item.Elapsed.HasValue && item.ProgressPercentage.HasValue ? " " : "")}{item.ProgressPercentage:P})" : "")} - {item.EventDescription}";
        }

        private void StartAliceProgressTracking(string taskId)
        {
            if (_aliceIsProgressTracking)
            {
                MessageBox.Show("Alice is already tracking progress.", "Error", MessageBoxButtons.OK);
                return;
            }

            _aliceIsProgressTracking = true;

            txtAliceTaskId.Text = taskId;

            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = SystemCommandName.GetCachedTaskProgressInfo,
                CommandRequestString = taskId
            };

            var progressWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            progressWorker.DoWork += aliceProgressWorker_DoWork;
            progressWorker.ProgressChanged += aliceProgressWorker_ProgressChanged;
            progressWorker.RunWorkerCompleted += aliceProgressWorker_RunWorkerCompleted;

            progressWorker.RunWorkerAsync(progressRequest);
        }

        private static void PopulateResponseSummary(ListControl lstTarget, HyperNodeMessageResponse response)
        {
            if (lstTarget == null)
                return;

            if (response == null)
            {
                lstTarget.DataSource = null;
                return;
            }

            lstTarget.DataSource = new[]
            {
                "Task ID: " + response.TaskId,
                "Responding Node Name:" + response.RespondingNodeName,
                "Total Run Time: " + response.TotalRunTime,
                "Node Action: " + response.NodeAction,
                "Node Action Reason: " + response.NodeActionReason,
                "Process Status Flags: " + response.ProcessStatusFlags,
                "Response XML: " + response.CommandResponseString,
                "Task Trace Count: " + response.TaskTrace.Count
            };
        }

        private static void PopulateTaskTrace(TreeView tvwTaskTrace, HyperNodeMessageResponse response)
        {
            if (tvwTaskTrace == null)
                return;

            if (response == null)
            {
                tvwTaskTrace.Nodes.Clear();
                return;
            }

            tvwTaskTrace.Nodes.AddRange(
                new[]
                {
                    new TreeNode(
                        response.RespondingNodeName,
                        response.TaskTrace.Select(i => new TreeNode(FormatActivityItem(i), string.IsNullOrWhiteSpace(i.EventDetail) ? new TreeNode[0] : new [] { new TreeNode(i.EventDetail) })).ToArray()
                    )
                    {
                        Tag = response
                    }
                }
            );
        }

        private void ClearResponseData()
        {
            // Clear out the real-time response data
            lstRealTimeResponse.DataSource = null;
            tvwRealTimeTaskTrace.Nodes.Clear();

            // Clear out Alice's response data
            lstAliceActivityItems.DataSource = null;
            lstAliceResponseSummary.DataSource = null;
            tvwAliceTaskTrace.Nodes.Clear();

            // Clear out Bob's response data
            lstBobActivityItems.DataSource = null;
            lstBobResponseSummary.DataSource = null;
            tvwBobTaskTrace.Nodes.Clear();
        }

        #endregion Private Methods
    }
}
