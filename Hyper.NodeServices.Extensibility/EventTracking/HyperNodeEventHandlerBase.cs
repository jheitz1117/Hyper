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
        public virtual void OnTaskStarted(ITaskStartedEventArgs args) { }
    }
}
