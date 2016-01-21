using System;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    internal sealed class MessageReceivedEventArgs : EventArgs, IMessageReceivedEventArgs
    {
        private readonly Action<string> _rejectMessageAction;

        public string HyperNodeName { get; private set; }

        public IReadOnlyHyperNodeMessageInfo MessageInfo
        {
            get { return _messageInfo; }
        } private readonly IReadOnlyHyperNodeMessageInfo _messageInfo;

        public MessageReceivedEventArgs(string hyperNodeName, IReadOnlyHyperNodeMessageInfo messageInfo, Action<string> rejectMessageAction)
        {
            this.HyperNodeName = hyperNodeName;
            _messageInfo = messageInfo;
            _rejectMessageAction = rejectMessageAction;
        }

        public void RejectMessage(string reason)
        {
            if (_rejectMessageAction != null)
                _rejectMessageAction(reason);
        }
    }
}
