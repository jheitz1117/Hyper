using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when the <see cref="IHyperNodeService"/> fires an event.
    /// </summary>
    public interface IHyperNodeEventArgs
    {
        /// <summary>
        /// Provides a way to raise activity events from inside an <see cref="IHyperNodeEventHandler"/>.
        /// </summary>
        ITaskActivityTracker Activity { get; }

        /// <summary>
        /// Contains information about the current task being executed.
        /// </summary>
        ITaskEventContext TaskContext { get; }
    }
}
