using System;

namespace Hyper.Services.HyperNodeServices
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
