using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal class HyperNodeEventArgs : EventArgs, IHyperNodeEventArgs
    {
        public ITaskActivityTracker Activity { get; }
        public ITaskEventContext TaskContext { get; }

        public HyperNodeEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext)
        {
            Activity = activity;
            TaskContext = taskContext;
        }
    }
}
