using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class HyperNodeRespondedEventArgs : HyperNodeEventArgs, IHyperNodeRespondedEventArgs
    {
        public string RespondingNodeName { get; }
        public IReadOnlyHyperNodeResponseInfo ResponseInfo { get; }

        public HyperNodeRespondedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, string respondingNodeName, IReadOnlyHyperNodeResponseInfo responseInfo)
            : base(activity, taskContext)
        {
            RespondingNodeName = respondingNodeName;
            ResponseInfo = responseInfo;
        }
    }
}
