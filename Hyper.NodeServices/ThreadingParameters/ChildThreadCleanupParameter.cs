using System;
using Hyper.NodeServices.ActivityTracking;

namespace Hyper.NodeServices
{
    internal class ChildThreadCleanupParameter
    {
        public IDisposable ToDispose { get; private set; }
        public HyperNodeServiceActivityTracker ActivityTracker { get; private set; }

        public ChildThreadCleanupParameter(IDisposable toDispose, HyperNodeServiceActivityTracker activityTracker)
        {
            this.ToDispose = toDispose;
            this.ActivityTracker = activityTracker;
        }
    }
}
