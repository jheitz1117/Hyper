using System.Threading;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandExecutionContext
    {
        ICommandModuleRequest Request { get; }
        ITaskActivityTracker Activity { get; }
        CancellationToken Token { get; }
    }
}
