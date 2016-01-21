using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class HyperNodeRespondedEventArgs : HyperNodeEventArgs, IHyperNodeRespondedEventArgs
    {
        public string RespondingNodeName { get; private set; }
        public IReadOnlyHyperNodeResponseInfo ResponseInfo { get; private set; }

        public HyperNodeRespondedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, string respondingNodeName, IReadOnlyHyperNodeResponseInfo responseInfo)
            : base(activity, taskContext)
        {
            this.RespondingNodeName = respondingNodeName;
            this.ResponseInfo = responseInfo;
        }
    }
}
