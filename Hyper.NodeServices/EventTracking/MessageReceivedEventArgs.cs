using System;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    internal sealed class MessageReceivedEventArgs : EventArgs, IMessageReceivedEventArgs
    {
        private readonly Action<string> _rejectMessageAction;

        public string HyperNodeName { get; private set; }

        public IHyperNodeMessageContext MessageContext
        {
            get { return _messageContext; }
        } private readonly IHyperNodeMessageContext _messageContext;

        public MessageReceivedEventArgs(string hyperNodeName, IHyperNodeMessageContext messageContext, Action<string> rejectMessageAction)
        {
            this.HyperNodeName = hyperNodeName;
            _messageContext = messageContext;
            _rejectMessageAction = rejectMessageAction;
        }

        public void RejectMessage(string reason)
        {
            if (_rejectMessageAction != null)
                _rejectMessageAction(reason);
        }
    }
}
