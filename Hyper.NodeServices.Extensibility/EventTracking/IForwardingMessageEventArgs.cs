using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> immediately before the <see cref="IHyperNodeService"/> forwards the current message to an adjacent recipient.
    /// This event fires once for each forwarding recipient.
    /// </summary>
    public interface IForwardingMessageEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/> to which the current message is about to be forwarded.
        /// </summary>
        string RecipientNodeName { get; }

        /// <summary>
        /// Causes the <see cref="IHyperNodeService"/> to cancel the task.
        /// </summary>
        void CancelTask();

        /// <summary>
        /// Causes the <see cref="IHyperNodeService"/> not to forward the current message to the current recipient.
        /// </summary>
        void SkipRecipient();
    }
}
