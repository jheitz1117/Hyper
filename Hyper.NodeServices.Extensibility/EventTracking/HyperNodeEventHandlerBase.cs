using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Concrete implementation of <see cref="IHyperNodeEventHandler"/> that handles each event by executing an empty block.
    /// Classes that derive from this class may override any method to execute custom code for that event.
    /// </summary>
    public class HyperNodeEventHandlerBase : IHyperNodeEventHandler
    {
        /// <summary>
        /// When overridden in a derived class, executes one-time initialization code while the <see cref="IHyperNodeService"/> is being configured.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// When overridden in a derived class, executes when a message is received by the <see cref="IHyperNodeService"/>.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnMessageReceived(IMessageReceivedEventArgs args) { }

        /// <summary>
        /// When overridden in a derived class, executes when a task is started by the <see cref="IHyperNodeService"/>.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnTaskStarted(IHyperNodeEventArgs args) { }

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> chooses to ignore a message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnMessageIgnored(IMessageIgnoredEventArgs args) { }

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes processing a message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnMessageProcessed(IHyperNodeEventArgs args) { }

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> determines that this is the first time it has processed the current message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnMessageSeen(IMessageSeenEventArgs args) { }

        /// <summary>
        /// Executes immediately before the <see cref="IHyperNodeService"/> forwards the current message to an adjacent recipient.
        /// This event fires once for each forwarding recipient.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnForwardingMessage(IForwardingMessageEventArgs args) { }

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> receives a <see cref="HyperNodeMessageResponse"/> from another <see cref="IHyperNodeService"/> to which the current message had been previously forwarded.
        /// This event only fires when the remote call was successfully executed and did not timeout or throw any faults or other exceptions.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnHyperNodeResponded(IHyperNodeRespondedEventArgs args) { }

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes executing the current task.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        public virtual void OnTaskCompleted(ITaskCompletedEventArgs args) { }
    }
}
