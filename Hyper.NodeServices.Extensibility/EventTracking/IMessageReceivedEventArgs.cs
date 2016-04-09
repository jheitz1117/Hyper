using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when a message is received by the <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IMessageReceivedEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// Causes the <see cref="IHyperNodeService"/> to reject the message.
        /// </summary>
        /// <param name="reason">The reason for rejecting the message.</param>
        void RejectMessage(string reason);
    }
}
