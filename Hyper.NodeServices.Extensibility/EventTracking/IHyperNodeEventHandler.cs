using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Defines a handler that subscribes to events raised by a <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IHyperNodeEventHandler
    {
        /// <summary>
        /// Provides the opportunity to run custom initialization code for <see cref="IHyperNodeEventHandler"/> implementations. This method is called
        /// immediately after the <see cref="IHyperNodeEventHandler"/> instance is instantiated. Once the <see cref="IHyperNodeEventHandler"/> has been initialized,
        /// it exists for the lifetime of the <see cref="IHyperNodeService"/>. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Executes when a message is received by the <see cref="IHyperNodeService"/>.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnMessageReceived(IMessageReceivedEventArgs args);

        /// <summary>
        /// Executes when a task is started by the <see cref="IHyperNodeService"/>.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnTaskStarted(IHyperNodeEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> chooses to ignore a message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnMessageIgnored(IMessageIgnoredEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes processing a message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnMessageProcessed(IHyperNodeEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> determines that this is the first time it has processed the current message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnMessageSeen(IMessageSeenEventArgs args);

        /// <summary>
        /// Executes immediately before the <see cref="IHyperNodeService"/> forwards the current message to an adjacent recipient.
        /// This event fires once for each forwarding recipient.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnForwardingMessage(IForwardingMessageEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> receives a <see cref="HyperNodeMessageResponse"/> from another <see cref="IHyperNodeService"/> to which the current message had been previously forwarded.
        /// This event only fires when the remote call was successfully executed and did not timeout or throw any faults or other exceptions.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnHyperNodeResponded(IHyperNodeRespondedEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes executing the current task.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnTaskCompleted(ITaskCompletedEventArgs args);
    }
}
