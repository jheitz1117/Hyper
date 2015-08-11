using System.Threading;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public interface ICommandExecutionContext : IHyperNodeMessageContext
    {
        string TaskId { get; }
        string ExecutingNodeName { get; }
        ICommandRequest Request { get; }
        ITaskActivityTracker Activity { get; }
        CancellationToken Token { get; }
    }
}
