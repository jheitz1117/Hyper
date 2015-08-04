using System;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    internal class ChildThreadCleanupParameter
    {
        public IDisposable ToDispose { get; private set; }
        public HyperNodeServiceActivityTracker ActivityTracker { get; private set; }
        public HyperNodeMessageResponse Response { get; private set; }
        
        public ChildThreadCleanupParameter(IDisposable toDispose, HyperNodeServiceActivityTracker activityTracker, HyperNodeMessageResponse response)
        {
            this.ToDispose = toDispose;
            this.ActivityTracker = activityTracker;
            this.Response = response;
        }
    }
}
