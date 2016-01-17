using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when a task is started by the <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface ITaskStartedEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// Causes the <see cref="IHyperNodeService"/> to cancel the task.
        /// </summary>
        void CancelTask();
    }
}
