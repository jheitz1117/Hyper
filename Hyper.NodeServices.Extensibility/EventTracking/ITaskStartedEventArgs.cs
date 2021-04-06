using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when the <see cref="IHyperNodeService"/> determines that this is the first time it has processed the current message.
    /// </summary>
    public interface ITaskStartedEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// Causes the <see cref="IHyperNodeService"/> to cancel the task.
        /// </summary>
        void CancelTask();
    }
}
