using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hyper.NodeServices.Client;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;

namespace HyperNodeTestClient
{
    public partial class MainForm : Form
    {
        private bool _bobIsProgressTracking;
        private bool _aliceIsProgressTracking;

        public MainForm()
        {
            InitializeComponent();

            tvwRealTimeTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
            tvwAliceTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
            tvwBobTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
        }

        #region Events

        private void btnDiscover_Click(object sender, EventArgs e)
        {
            try
            {
                cboHyperNodeNames.DataSource = null;

                var serializer = new NetDataContractResponseSerializer<DiscoverResponse>();
                var msg = new HyperNodeMessageRequest("HyperNodeTestClient")
                {
                    CommandName = SystemCommandNames.Discover
                };

                var response = new HyperNodeClient("Alice").ProcessMessage(msg);

                if (!string.IsNullOrWhiteSpace(response.CommandResponseString))
                {
                    var aliceDiscoverResponse = ((ICommandResponseSerializer)serializer).Deserialize(response.CommandResponseString) as DiscoverResponse;
                    if (aliceDiscoverResponse != null)
                    {
                        cboHyperNodeNames.DataSource = new[]
                        {
                            new []
                            {
                                "Alice"
                            },
                            aliceDiscoverResponse.ChildNodes.Keys
                        }.SelectMany(l => l)
                        .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefreshCommandList_Click(object sender, EventArgs e)
        {
            try
            {
                cboCommandNames.DataSource = null;

                var serializer = new NetDataContractResponseSerializer<GetCommandConfigResponse>();
                var msg = new HyperNodeMessageRequest("HyperNodeTestClient")
                {
                    CommandName = SystemCommandNames.GetCommandConfig,
                    IntendedRecipientNodeNames = new List<string>
                    {
                        cboHyperNodeNames.Text
                    },
                    ForwardingPath = GetForwardingPathFromAliceToBob()
                };

                var response = new HyperNodeClient("Alice").ProcessMessage(msg);

                // TODO: Recursively find the response we're actually interested in

                if (!string.IsNullOrWhiteSpace(response.CommandResponseString))
                {
                    var commandResponse =
                        ((ICommandResponseSerializer)serializer).Deserialize(response.CommandResponseString) as
                            GetCommandConfigResponse;
                    if (commandResponse != null)
                        cboCommandNames.DataSource =
                            commandResponse.CommandConfigurations.Select(c => c.CommandName).ToList();
                }
                else
                {
                    MessageBox.Show(response.RespondingNodeName + " did not send back a command response string.");
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
                // Clear out our datasource first
                ClearResponseData();

                // Create our message request
                var serializer = new NetDataContractRequestSerializer<EnableActivityMonitorRequest>();
                var msg = new HyperNodeMessageRequest("HyperNodeTestClient")
                {
                    CommandName = cboCommandNames.Text,
                    CommandRequestString = serializer.Serialize(
                        new EnableActivityMonitorRequest
                        {
                            ActivityMonitorName = "TaskActivityTracer",
                            Enable = true
                        }
                    ),
                    IntendedRecipientNodeNames = new List<string>
                    {
                        cboHyperNodeNames.Text
                    },
                    ProcessOptionFlags = (chkReturnTaskTrace.Checked ? MessageProcessOptionFlags.ReturnTaskTrace : MessageProcessOptionFlags.None) |
                                         (chkRunConcurrently.Checked ? MessageProcessOptionFlags.RunConcurrently : MessageProcessOptionFlags.None) |
                                         (chkCacheProgressInfo.Checked ? MessageProcessOptionFlags.CacheProgressInfo : MessageProcessOptionFlags.None)
                };

                if (cboHyperNodeNames.Text == "Bob")
                    msg.ForwardingPath = GetForwardingPathFromAliceToBob();

                txtMessageId.Text = msg.MessageGuid.ToString();

                var response = new HyperNodeClient("Alice").ProcessMessage(msg);

                PopulateResponseSummary(lstRealTimeResponse, response);
                PopulateTaskTrace(tvwRealTimeTaskTrace, response);

                if (msg.CacheProgressInfo)
                    StartAliceProgressTracking(msg.MessageGuid, response.TaskId);
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

            var alice = new HyperNodeClient("Alice");
            var taskProgressInfo = new HyperNodeTaskProgressInfo();
            ICommandResponseSerializer serializer = new NetDataContractResponseSerializer<HyperNodeTaskProgressInfo>();

            while (!taskProgressInfo.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
            {
                var aliceProgress = alice.ProcessMessage((HyperNodeMessageRequest)e.Argument);
                if (aliceProgress == null)
                    break;

                var targetProgress = aliceProgress;
                if (string.IsNullOrWhiteSpace(targetProgress.CommandResponseString))
                    break;

                taskProgressInfo = (HyperNodeTaskProgressInfo)serializer.Deserialize(targetProgress.CommandResponseString);

                ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(taskProgressInfo.ProgressPercentage), taskProgressInfo);

                Task.Delay(500).Wait();
            }

            progressTimer.Stop();

            e.Result = taskProgressInfo;
        }

        private void aliceProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var taskProgressInfo = ((HyperNodeTaskProgressInfo)e.UserState);

            lstAliceActivityItems.DataSource = GetActivityStrings(taskProgressInfo.Activity);

            if (taskProgressInfo.ChildTaskIds.ContainsKey("Bob") && !_bobIsProgressTracking && cboHyperNodeNames.Text == "Bob")
                StartBobProgressTracking(taskProgressInfo.ParentMessageGuid, taskProgressInfo.ChildTaskIds["Bob"]);
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

        private static void bobProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var progressTimer = new Stopwatch();
            progressTimer.Start();

            var alice = new HyperNodeClient("Alice");
            var taskProgressInfo = new HyperNodeTaskProgressInfo();
            ICommandResponseSerializer serializer = new NetDataContractResponseSerializer<HyperNodeTaskProgressInfo>();

            while (!taskProgressInfo.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
            {
                var aliceProgress = alice.ProcessMessage((HyperNodeMessageRequest)e.Argument);
                if (aliceProgress == null || !aliceProgress.ChildResponses.ContainsKey("Bob"))
                    break;

                var targetProgress = aliceProgress.ChildResponses["Bob"];
                if (string.IsNullOrWhiteSpace(targetProgress.CommandResponseString))
                    break;

                taskProgressInfo = (HyperNodeTaskProgressInfo)serializer.Deserialize(targetProgress.CommandResponseString);

                ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(taskProgressInfo.ProgressPercentage), taskProgressInfo);

                Task.Delay(500).Wait();
            }

            progressTimer.Stop();

            e.Result = taskProgressInfo;
        }

        private void bobProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lstBobActivityItems.DataSource = GetActivityStrings(((HyperNodeTaskProgressInfo)e.UserState).Activity);
        }

        private void bobProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Bob's Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var taskProgressInfo = (HyperNodeTaskProgressInfo)e.Result;

                lstBobActivityItems.DataSource = GetActivityStrings(taskProgressInfo.Activity);
                PopulateResponseSummary(lstBobResponseSummary, taskProgressInfo.Response);
                PopulateTaskTrace(tvwBobTaskTrace, taskProgressInfo.Response);
            }

            _bobIsProgressTracking = false;
        }

        private static void tvwTaskTrace_OnBeforeExpand(object sender, TreeViewCancelEventArgs treeViewCancelEventArgs)
        {
            var expandingNode = treeViewCancelEventArgs.Node;
            var expandingResponse = expandingNode.Tag as ConcurrentDictionary<string, HyperNodeMessageResponse>;

            if (expandingResponse != null)
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
                            },
                            new TreeNode(
                                "Child Responses", 
                                kvp.Value.ChildResponses.Select(
                                    r=> new TreeNode(r.Key)
                                    {
                                        Tag = r.Value
                                    }
                                ).ToArray()
                            )
                            {
                                Tag = kvp.Value.ChildResponses
                            }
                        }
                    ).ToArray()
                );
            }
        }

        #endregion Events

        #region Private Methods

        private static HyperNodePath GetForwardingPathFromAliceToBob()
        {
            var path = new HyperNodePath();

            // TODO: Implement BreadthFirstSearch to find consolidated path for all intended recipients

            path.PathTree.TryAdd("Alice",
                new List<HyperNodeVertex>
                {
                    new HyperNodeVertex
                    {
                        Key = "Bob"
                    }
                }
            );

            return path;
        }

        private static string[] GetActivityStrings(IEnumerable<HyperNodeActivityItem> activity)
        {
            return (
                from a in activity
                orderby a.EventDateTime
                select string.Format("{0:G} {1} ({2:P}) - {3}", a.EventDateTime, a.Agent, a.ProgressPercentage, a.EventDescription)
            ).ToArray();
        }

        private void StartAliceProgressTracking(Guid messageGuid, string taskId)
        {
            if (_aliceIsProgressTracking)
            {
                MessageBox.Show("Alice is already tracking progress.", "Error", MessageBoxButtons.OK);
                return;
            }

            _aliceIsProgressTracking = true;

            txtAliceTaskId.Text = taskId;

            var serializer = new NetDataContractRequestSerializer<GetCachedTaskProgressInfoRequest>();
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = SystemCommandNames.GetCachedTaskProgressInfo,
                CommandRequestString = serializer.Serialize(
                    new GetCachedTaskProgressInfoRequest
                    {
                        MessageGuid = messageGuid,
                        TaskId = taskId
                    }
                )
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

        private void StartBobProgressTracking(Guid messageGuid, string taskId)
        {
            if (_bobIsProgressTracking)
            {
                MessageBox.Show("Bob is already tracking progress.", "Error", MessageBoxButtons.OK);
                return;
            }

            _bobIsProgressTracking = true;

            txtBobTaskId.Text = taskId;

            var serializer = new NetDataContractRequestSerializer<GetCachedTaskProgressInfoRequest>();
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = SystemCommandNames.GetCachedTaskProgressInfo,
                CommandRequestString = serializer.Serialize(
                    new GetCachedTaskProgressInfoRequest
                    {
                        MessageGuid = messageGuid,
                        TaskId = taskId
                    }
                ),
                IntendedRecipientNodeNames = new List<string>
                {
                    "Bob"
                },
                ForwardingPath = GetForwardingPathFromAliceToBob()
            };

            var progressWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            progressWorker.DoWork += bobProgressWorker_DoWork;
            progressWorker.ProgressChanged += bobProgressWorker_ProgressChanged;
            progressWorker.RunWorkerCompleted += bobProgressWorker_RunWorkerCompleted;

            progressWorker.RunWorkerAsync(progressRequest);
        }

        private static void PopulateResponseSummary(ListControl lstTarget, HyperNodeMessageResponse response)
        {
            if (lstTarget == null)
            {
                return;
            }
            if (response == null)
            {
                lstTarget.DataSource = null;
                return;
            }

            lstTarget.DataSource = new[]
            {
                "Task ID: " + response.TaskId,
                "Responding Node Name:" + response.RespondingNodeName,
                "Node Action: " + response.NodeAction,
                "Node Action Reason: " + response.NodeActionReason,
                "Process Status Flags: " + response.ProcessStatusFlags,
                "Child Response Count: " + response.ChildResponses.Count,
                "Response XML: " + response.CommandResponseString,
                "Task Trace Count: " + response.TaskTrace.Count
            };
        }

        private static void PopulateTaskTrace(TreeView tvwTaskTrace, HyperNodeMessageResponse response)
        {
            if (tvwTaskTrace == null)
            {
                return;
            }
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
                        GetActivityStrings(response.TaskTrace).Select(s=>new TreeNode(s)).ToArray()
                    )
                    {
                        Tag = response
                    },
                    new TreeNode(
                        "Child Responses", 
                        response.ChildResponses.Select(
                            r=> new TreeNode(r.Key)
                            {
                                Tag = r.Value
                            }
                        ).ToArray()
                    )
                    {
                        Tag = response.ChildResponses
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
