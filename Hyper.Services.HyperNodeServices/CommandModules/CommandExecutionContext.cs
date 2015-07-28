using System.Threading;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices.CommandModules
{
    internal class CommandExecutionContext : ICommandExecutionContext
    {
        public ICommandModuleRequest Request { get; set; }
        public ITaskActivityTracker Activity { get; set; }
        public CancellationToken Token { get; set; }
    }
}
