using System;
using Hyper.NodeServices.EventTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    internal class TaskActivityEventContext : HyperNodeEventContext
    {
        private readonly HyperNodeEventTracker _eventTracker;
        private readonly string _commandName;
        private readonly string _taskId;
        private readonly Action _cancelTaskAction;

        public HyperNodeEventTracker EventTracker { get { return _eventTracker; } }
        public string CommandName { get { return _commandName; } }
        public string TaskId { get { return _taskId; } }
        public Action CancelTaskAction { get { return _cancelTaskAction; } }

        public TaskActivityEventContext(string hyperNodeName, HyperNodeEventTracker eventTracker, string commandName, string taskId, bool enableDiagnostics, Action cancelTaskAction)
            : base(hyperNodeName, enableDiagnostics)
        {
            _eventTracker = eventTracker;
            _commandName = commandName;
            _taskId = taskId;
            _cancelTaskAction = cancelTaskAction;
        }
    }
}
