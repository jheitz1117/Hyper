using Hyper.NodeServices.Contracts;

namespace Hyper.Services.HyperNodeServices
{
    internal class ForwardingTaskParameter
    {
        public string HyperNodeName { get; private set; }
        public HyperNodeServiceActivityTracker ActivityTracker { get; private set; }
        public HyperNodeMessageRequest Message { get; private set; }

        public ForwardingTaskParameter(string hyperNodeName, HyperNodeServiceActivityTracker activityTracker, HyperNodeMessageRequest message)
        {
            this.HyperNodeName = hyperNodeName;
            this.ActivityTracker = activityTracker;
            this.Message = message;
        }
    }
}
