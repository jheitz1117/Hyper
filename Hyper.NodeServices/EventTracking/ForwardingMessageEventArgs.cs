using System;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class ForwardingMessageEventArgs : HyperNodeEventArgs, IForwardingMessageEventArgs
    {
        private readonly string _recipientNodeName;
        private readonly Action _cancelTaskAction;
        private readonly Action _skipRecipientAction;

        public string RecipientNodeName
        {
            get { return _recipientNodeName; }
        }

        public ForwardingMessageEventArgs(ITaskActivityTracker activity, ITaskEventContext taskContext, string recipientNodeName, Action cancelTaskAction, Action skipRecipientAction)
            : base(activity, taskContext)
        {
            _recipientNodeName = recipientNodeName;
            _cancelTaskAction = cancelTaskAction;
            _skipRecipientAction = skipRecipientAction;
        }

        public void CancelTask()
        {
            if (_cancelTaskAction != null)
                _cancelTaskAction();
        }

        public void SkipRecipient()
        {
            if (_skipRecipientAction != null)
                _skipRecipientAction();
        }
    }
}
