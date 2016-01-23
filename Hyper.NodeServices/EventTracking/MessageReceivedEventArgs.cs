using System;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    internal sealed class MessageReceivedEventArgs : EventArgs, IMessageReceivedEventArgs
    {
        private readonly Action<string> _rejectMessageAction;

        public string HyperNodeName { get; }

        public IReadOnlyHyperNodeMessageInfo MessageInfo { get; }

        public MessageReceivedEventArgs(string hyperNodeName, IReadOnlyHyperNodeMessageInfo messageInfo, Action<string> rejectMessageAction)
        {
            HyperNodeName = hyperNodeName;
            MessageInfo = messageInfo;
            _rejectMessageAction = rejectMessageAction;
        }

        public void RejectMessage(string reason)
        {
            _rejectMessageAction?.Invoke(reason);
        }
    }
}
