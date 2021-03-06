﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class TaskCompletedEventArgs : HyperNodeEventArgs, ITaskCompletedEventArgs
    {
        public IReadOnlyHyperNodeResponseInfo ResponseInfo { get; }

        public TaskCompletedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, IReadOnlyHyperNodeResponseInfo responseInfo)
            : base(activity, taskContext)
        {
            ResponseInfo = responseInfo;
        }
    }
}
