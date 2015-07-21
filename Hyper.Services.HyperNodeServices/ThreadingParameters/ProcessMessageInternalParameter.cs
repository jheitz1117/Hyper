using Hyper.Services.HyperNodeActivityTracking;
using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeServices
{
    internal class ProcessMessageInternalParameter
    {
        public HyperNodeMessageRequest Message { get; private set; }
        public HyperNodeMessageResponse Response { get; private set; }
        public HyperNodeTaskActivityTracker ActivityTracker { get; private set; }

        public ProcessMessageInternalParameter(HyperNodeMessageRequest message, HyperNodeMessageResponse response, HyperNodeTaskActivityTracker activityTracker)
        {
            this.Message = message;
            this.Response = response;
            this.ActivityTracker = activityTracker;
        }
    }
}
