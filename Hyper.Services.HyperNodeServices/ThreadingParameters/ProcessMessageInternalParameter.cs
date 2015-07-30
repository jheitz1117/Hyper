using System.Threading;
using Hyper.NodeServices.Contracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal class ProcessMessageInternalParameter
    {
        public HyperNodeMessageRequest Message { get; private set; }
        public HyperNodeMessageResponse Response { get; private set; }
        public ITaskActivityTracker ActivityTracker { get; private set; }
        public CancellationToken Token { get; private set; }

        public ProcessMessageInternalParameter(HyperNodeMessageRequest message, HyperNodeMessageResponse response, ITaskActivityTracker activityTracker, CancellationToken token)
        {
            this.Message = message;
            this.Response = response;
            this.ActivityTracker = activityTracker;
            this.Token = token;
        }
    }
}
