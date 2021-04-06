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
        void OnTaskStarted(ITaskStartedEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes processing a message.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnMessageProcessed(IHyperNodeEventArgs args);

        /// <summary>
        /// Executes when the <see cref="IHyperNodeService"/> finishes executing the current task.
        /// </summary>
        /// <param name="args">The event arguments passed from the <see cref="IHyperNodeService"/>.</param>
        void OnTaskCompleted(ITaskCompletedEventArgs args);
    }
}
