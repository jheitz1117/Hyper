using System.Threading;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    internal class ProcessMessageInternalParameter
    {
        public HyperNodeMessageRequest Message { get; private set; }
        public HyperNodeMessageResponse Response { get; private set; }
        public HyperNodeServiceActivityTracker ActivityTracker { get; private set; }
        public CancellationToken Token { get; private set; }

        public ProcessMessageInternalParameter(HyperNodeMessageRequest message, HyperNodeMessageResponse response, HyperNodeServiceActivityTracker activityTracker, CancellationToken token)
        {
            this.Message = message;
            this.Response = response;
            this.ActivityTracker = activityTracker;
            this.Token = token;
        }
    }
}
