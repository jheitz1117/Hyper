using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class TaskStartedEventArgs : HyperNodeEventArgs, ITaskStartedEventArgs
    {
        private readonly Action _cancelAction;

        public TaskStartedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, Action cancelAction)
            :base(activity, taskContext)
        {
            _cancelAction = cancelAction;
        }

        public void CancelTask()
        {
            if (_cancelAction != null)
                _cancelAction();
        }
    }
}
