using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class MessageReceivedEventArgs : HyperNodeEventArgs, IMessageReceivedEventArgs
    {
        private readonly Action<string> _rejectMessageAction;

        public MessageReceivedEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, Action<string> rejectMessageAction)
            : base(activity, taskContext)
        {
            _rejectMessageAction = rejectMessageAction;
        }

        public void RejectMessage(string reason)
        {
            _rejectMessageAction?.Invoke(reason);
        }
    }
}
