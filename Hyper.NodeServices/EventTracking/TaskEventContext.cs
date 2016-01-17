using System;
using System.Diagnostics;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal class TaskEventContext : ITaskEventContext
    {
        private readonly HyperNodeMessageRequest _message;
        private readonly string _hyperNodeName;
        private readonly Stopwatch _stopwatch;
        private readonly string _taskId;

        public string HyperNodeName { get { return _hyperNodeName; } }
        public string CommandName { get { return _message.CommandName; } }
        public string TaskId { get { return _taskId; } }
        public IReadOnlyHyperNodeMessageInfo MessageInfo
        {
            get
            {
                // Return a new message context each time, since the message may change over time
                return new ReadOnlyHyperNodeMessageInfo(_message.IntendedRecipientNodeNames, _message.SeenByNodeNames)
                {
                    CommandName = _message.CommandName,
                    CreatedByAgentName = _message.CreatedByAgentName,
                    CreationDateTime = _message.CreationDateTime,
                    ProcessOptionFlags = _message.ProcessOptionFlags
                };
            }
        }

        public TimeSpan? Elapsed
        {
            get
            {
                TimeSpan? elapsed = null;

                if (_stopwatch != null)
                {
                    elapsed = _stopwatch.Elapsed;

                    // The reason we don't start the stopwatch *before* reporting the elapsed time is that we always want the very first
                    // call to Elapsed to return 00:00:00
                    if (!_stopwatch.IsRunning)
                        _stopwatch.Start();
                }

                return elapsed;
            }
        }

        public TaskEventContext(string hyperNodeName, HyperNodeMessageRequest message, string taskId, bool enableDiagnostics)
        {
            _hyperNodeName = hyperNodeName;
            _message = message;
            _taskId = taskId;

            if (enableDiagnostics)
                _stopwatch = new Stopwatch();
        }
    }
}
