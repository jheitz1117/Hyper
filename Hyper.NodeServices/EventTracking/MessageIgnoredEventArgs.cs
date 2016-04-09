using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class MessageIgnoredEventArgs : HyperNodeEventArgs, IMessageIgnoredEventArgs
    {
        public string Reason { get; }

        public MessageIgnoredEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, string reason)
            : base(activity, taskContext)
        {
            Reason = reason;
        }
    }
}
