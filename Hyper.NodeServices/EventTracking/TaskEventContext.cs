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
        private readonly Stopwatch _stopwatch;

        public string HyperNodeName { get; }
        public string TaskId { get; }
        public string CommandName => _message.CommandName;

        public IReadOnlyHyperNodeMessageInfo MessageInfo => new ReadOnlyHyperNodeMessageInfo(_message);

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
            _message = message;

            HyperNodeName = hyperNodeName;
            TaskId = taskId;

            if (enableDiagnostics)
                _stopwatch = new Stopwatch();
        }
    }
}
