using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class ForwardingMessageEventArgs : HyperNodeEventArgs, IForwardingMessageEventArgs
    {
        private readonly Action _cancelTaskAction;
        private readonly Action _skipRecipientAction;

        public string RecipientNodeName { get; }

        public ForwardingMessageEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, string recipientNodeName, Action cancelTaskAction, Action skipRecipientAction)
            : base(activity, taskContext)
        {
            RecipientNodeName = recipientNodeName;
            _cancelTaskAction = cancelTaskAction;
            _skipRecipientAction = skipRecipientAction;
        }

        public void CancelTask()
        {
            _cancelTaskAction?.Invoke();
        }

        public void SkipRecipient()
        {
            _skipRecipientAction?.Invoke();
        }
    }
}
