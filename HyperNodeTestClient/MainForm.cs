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
using HyperNet.ExtensibilityTest.Shared.CommandModules;

namespace HyperNodeTestClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Events

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
            lstAliceProgress.DataSource = GetActivityStrings(((HyperNodeTaskProgressInfo)e.UserState).Activity);
        }

        private void aliceProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var taskProgressInfo = (HyperNodeTaskProgressInfo)e.Result;

                lstAliceProgress.DataSource = GetActivityStrings(taskProgressInfo.Activity);
                PopulateResponse(lstFinalAliceResponse, taskProgressInfo.Response);

                StartBobProgressTracking(taskProgressInfo.ParentMessageGuid, taskProgressInfo.Response.ChildResponses["Bob"].TaskId);
            }
        }

        private static void bobProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var progressTimer = new Stopwatch();
            progressTimer.Start();

            var alice = new HyperNodeClient("Alice");
            var progressResponse = new HyperNodeTaskProgressInfo();
            ICommandResponseSerializer serializer = new NetDataContractResponseSerializer<HyperNodeTaskProgressInfo>();
            while (!progressResponse.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
            {
                var aliceProgress = alice.ProcessMessage((HyperNodeMessageRequest)e.Argument);
                if (aliceProgress == null || !aliceProgress.ChildResponses.ContainsKey("Bob"))
                    break;

                var targetProgress = aliceProgress.ChildResponses["Bob"];
                if (string.IsNullOrWhiteSpace(targetProgress.CommandResponseString))
                    break;

                progressResponse = (HyperNodeTaskProgressInfo)serializer.Deserialize(targetProgress.CommandResponseString);

                ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(progressResponse.ProgressPercentage), progressResponse);

                Task.Delay(500).Wait();
            }

            progressTimer.Stop();

            e.Result = progressResponse;
        }

        private void bobProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lstBobProgress.DataSource = GetActivityStrings(((HyperNodeTaskProgressInfo)e.UserState).Activity);
        }

        private void bobProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var progressInfo = (HyperNodeTaskProgressInfo)e.Result;

                lstBobProgress.DataSource = GetActivityStrings(progressInfo.Activity);
                PopulateResponse(lstFinalBobResponse, progressInfo.Response);
            }
        }

        private void btnToBobViaAlice_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear out our datasource first
                ClearResponseData();

                var alice = new HyperNodeClient("Alice");

                var serializer = new DataContractCommandSerializer<ComplexCommandRequest, ComplexCommandResponse>();

                var msg = new HyperNodeMessageRequest("HyperNodeTestClient")
                {
                    CommandName = "LongRunningTaskTest",
                    CommandRequestString = serializer.Serialize(
                        new ComplexCommandRequest
                        {
                            MyString = "The magic string",
                            MyDateTime = DateTime.Now,
                            MyInt32 = 100,
                            MyTimeSpan = TimeSpan.FromHours(50)
                        }
                    ),
                    //CommandName = "ComplexCommand",
                    //CommandName = "SuperLongRunningTestTask",
                    //CommandName = "LongRunningTaskTest",
                    //CommandName = "ValidCommand",
                    IntendedRecipientNodeNames = new List<string>
                    {
                        "Bob"
                    },
                    ForwardingPath = GetForwardingPath("Alice", "Bob"),
                    ForwardingTimeout = new TimeSpan(0, 0, 5),
                    MessageLifeSpan = new TimeSpan(1, 0, 0), // long running command needs a lifespan of longer than the default
                    ProcessOptionFlags = (chkReturnTaskTrace.Checked ? MessageProcessOptionFlags.ReturnTaskTrace : MessageProcessOptionFlags.None) |
                                         (chkRunConcurrently.Checked ? MessageProcessOptionFlags.RunConcurrently : MessageProcessOptionFlags.None) |
                                         (chkCacheProgressInfo.Checked ? MessageProcessOptionFlags.CacheProgressInfo : MessageProcessOptionFlags.None)
                };

                var response = alice.ProcessMessage(msg);
                PopulateResponse(lstRealTimeResponse, response);

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
                if (msg.CacheProgressInfo)
                {
                    lblAliceProgress.Text = string.Format("Alice Progress (Message GUID {0})", msg.MessageGuid);
                    lblBobProgress.Text = string.Format("Bob Progress (Message GUID {0})", msg.MessageGuid);

                    StartAliceProgressTracking(msg.MessageGuid, response.TaskId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tvwTaskTrace_OnBeforeExpand(object sender, TreeViewCancelEventArgs treeViewCancelEventArgs)
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

        private static HyperNodePath GetForwardingPath(string fromNode, string toNode)
        {
            var path = new HyperNodePath();

            // TODO: Implement BreadthFirstSearch to find consolidated path for all intended recipients

            path.PathTree.TryAdd("Alice",
                new List<HyperNodeVertex>
                {
                    new HyperNodeVertex
                    {
                        Key = "Bob"
                    },
                    new HyperNodeVertex
                    {
                        Key="John"
                    }
                }
            );

            path.PathTree.TryAdd("Bob",
                new List<HyperNodeVertex>
                {
                    new HyperNodeVertex
                    {
                        Key = "Alice"
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
            var serializer = new NetDataContractRequestSerializer<GetCachedTaskProgressInfoRequest>();
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = "GetCachedTaskProgressInfo",
                CommandRequestString = serializer.Serialize(
                    new GetCachedTaskProgressInfoRequest
                    {
                        MessageGuid = messageGuid,
                        TaskId = taskId
                    }
                ),
                IntendedRecipientNodeNames = new List<string>
                {
                    "Alice"
                },
                ForwardingPath = GetForwardingPath("Alice", "Bob"),
                ForwardingTimeout = new TimeSpan(0, 0, 5),
                MessageLifeSpan = new TimeSpan(1, 0, 0)
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
            var serializer = new NetDataContractRequestSerializer<GetCachedTaskProgressInfoRequest>();
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = "GetCachedTaskProgressInfo",
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
                ForwardingPath = GetForwardingPath("Alice", "Bob"),
                ForwardingTimeout = new TimeSpan(0, 0, 5),
                MessageLifeSpan = new TimeSpan(1, 0, 0)
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

        private static void PopulateResponse(ListControl lstTarget, HyperNodeMessageResponse response)
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

        private void ClearResponseData()
        {
            tvwTaskTrace.Nodes.Clear();
            lstAliceProgress.DataSource = null;
            lstRealTimeResponse.DataSource = null;
            lstBobProgress.DataSource = null;
            lstFinalAliceResponse.DataSource = null;
            lstFinalBobResponse.DataSource = null;
        }

        #endregion Private Methods
    }
}
