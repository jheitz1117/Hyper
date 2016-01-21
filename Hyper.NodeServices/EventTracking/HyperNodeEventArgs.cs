using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal class HyperNodeEventArgs : EventArgs, IHyperNodeEventArgs
    {
        public ITaskActivityTracker Activity { get; private set; }
        public ITaskEventContext TaskContext { get; private set; }

        public HyperNodeEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext)
        {
            this.Activity = activity;
            this.TaskContext = taskContext;
        }
    }
}
