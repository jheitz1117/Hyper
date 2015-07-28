using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeProxies;
using HyperNetExtensibilityTest.CommandModules;

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
            var progressResponse = new HyperNodeProgressInfo();
            while (!progressResponse.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
            {
                var aliceProgress = alice.ProcessMessage((HyperNodeMessageRequest)e.Argument);
                if (aliceProgress == null)
                    break;

                //var targetProgress = aliceProgress.ChildResponses["Bob"];
                var targetProgress = aliceProgress;
                if (string.IsNullOrWhiteSpace(targetProgress.CommandResponseString))
                    break;

                progressResponse = (HyperNodeProgressInfo)new DataContractSerializer(
                    typeof(HyperNodeProgressInfo),
                    new[]
                        {
                            typeof (List<HyperNodeActivityItem>),
                            typeof (HyperNodeActivityItem)
                        }
                ).ReadObject(
                    new XmlTextReader(
                        new StringReader(targetProgress.CommandResponseString)
                        )
                );

                ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(progressResponse.ProgressPercentage), progressResponse);

                Task.Delay(500).Wait();
            }

            progressTimer.Stop();

            e.Result = progressResponse;
        }

        private void aliceProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lstAliceProgress.DataSource = GetActivityStrings(((HyperNodeProgressInfo)e.UserState).Activity);
        }

        private void aliceProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lstAliceProgress.DataSource = GetActivityStrings(((HyperNodeProgressInfo)e.Result).Activity);
            }
        }

        private static void bobProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var progressTimer = new Stopwatch();
            progressTimer.Start();

            var alice = new HyperNodeClient("Alice");
            var progressResponse = new HyperNodeProgressInfo();
            while (!progressResponse.IsComplete && progressTimer.Elapsed <= TimeSpan.FromMinutes(2))
            {
                var aliceProgress = alice.ProcessMessage((HyperNodeMessageRequest)e.Argument);
                if (aliceProgress == null || !aliceProgress.ChildResponses.ContainsKey("Bob"))
                    break;

                var targetProgress = aliceProgress.ChildResponses["Bob"];
                if (string.IsNullOrWhiteSpace(targetProgress.CommandResponseString))
                    break;

                progressResponse = (HyperNodeProgressInfo)new DataContractSerializer(
                    typeof(HyperNodeProgressInfo),
                    new[]
                        {
                            typeof (List<HyperNodeActivityItem>),
                            typeof (HyperNodeActivityItem)
                        }
                ).ReadObject(
                    new XmlTextReader(
                        new StringReader(targetProgress.CommandResponseString)
                        )
                );

                ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(progressResponse.ProgressPercentage), progressResponse);

                Task.Delay(500).Wait();
            }

            progressTimer.Stop();

            e.Result = progressResponse;
        }

        private void bobProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lstBobProgress.DataSource = GetActivityStrings(((HyperNodeProgressInfo)e.UserState).Activity);
        }

        private void bobProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Error Getting Progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                lstBobProgress.DataSource = GetActivityStrings(((HyperNodeProgressInfo)e.Result).Activity);
            }
        }

        private void btnToBobViaAlice_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear out our datasource first
                tvwTaskTrace.Nodes.Clear();
                lstAliceProgress.DataSource = null;
                lstResponse.DataSource = null;

                var alice = new HyperNodeClient("Alice");

                var serializer = new DataContractCommandSerializer<ComplexCommandRequest, ComplexCommandResponse>();
                
                var msg = new HyperNodeMessageRequest("HyperNodeTestClient")
                {
                    CommandName = "ComplexCommand",
                    CommandRequestString = serializer.Serialize(
                        new ComplexCommandRequest
                        {
                            MyString = "The magic string",
                            MyDateTime = DateTime.Now,
                            MyInt32 = 100,
                            MyTimeSpan = TimeSpan.FromHours(50)
                        }
                    ),
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
                    ReturnTaskTrace = chkReturnTaskTrace.Checked,
                    RunConcurrently = chkRunConcurrently.Checked,
                    CacheProgressInfo = chkCacheProgressInfo.Checked
                };

                var response = alice.ProcessMessage(msg);
                lstResponse.DataSource = new[]
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

                    StartAliceProgressTracking(msg.MessageGuid);
                    StartBobProgressTracking(msg.MessageGuid);
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

        private void StartAliceProgressTracking(Guid messageGuid)
        {
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = "GetCachedProgressInfo",
                CommandRequestString = messageGuid.ToString(),
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

        private void StartBobProgressTracking(Guid messageGuid)
        {
            var progressRequest = new HyperNodeMessageRequest("HyperNodeTestClient")
            {
                CommandName = "GetCachedProgressInfo",
                CommandRequestString = messageGuid.ToString(),
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

        #endregion Private Methods
    }
}
