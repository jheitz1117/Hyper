using System;
using System.Diagnostics;

namespace Hyper.NodeServices.ActivityTracking
{
    internal class HyperNodeActivityContext
    {
        private readonly string _hyperNodeName;
        private readonly Guid _messageGuid;
        private readonly string _commandName;
        private readonly string _taskId;
        private readonly Stopwatch _stopwatch;

        public string HyperNodeName { get { return _hyperNodeName; } }
        public Guid MessageGuid { get { return _messageGuid; } }
        public string CommandName { get { return _commandName; } }
        public string TaskId { get { return _taskId; } }

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

        public HyperNodeActivityContext(string hyperNodeName, Guid messageGuid, string commandName, string taskId, bool enableDiagnostics)
        {
            _hyperNodeName = hyperNodeName;
            _messageGuid = messageGuid;
            _commandName = commandName;
            _taskId = taskId;

            if (enableDiagnostics)
                _stopwatch = new Stopwatch();
        }
    }
}
