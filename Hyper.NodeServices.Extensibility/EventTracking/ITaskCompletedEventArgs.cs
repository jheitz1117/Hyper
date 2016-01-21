using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when the <see cref="IHyperNodeService"/> finishes executing the current task.
    /// </summary>
    public interface ITaskCompletedEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// The finished response for the current task.
        /// </summary>
        IReadOnlyHyperNodeResponseInfo ResponseInfo { get; }
    }
}
