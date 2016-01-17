using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    // TODO: Comment these!
    public interface IHyperNodeEventArgs
    {
        ITaskActivityTracker Activity { get; }
        ITaskEventContext TaskContext { get; }
    }
}
