using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class TaskStartedEventArgs : HyperNodeEventArgs, ITaskStartedEventArgs
    {
        private readonly Action _cancelTaskAction;

        public TaskStartedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, Action cancelTaskAction)
            :base(activity, taskContext)
        {
            _cancelTaskAction = cancelTaskAction;
        }

        public void CancelTask()
        {
            _cancelTaskAction?.Invoke();
        }
    }
}
