using System.Linq;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetNodeStatusCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            context.Activity.Track("Retrieving HyperNode status info.");

            return new GetNodeStatusResponse
            {
                DiagnosticsEnabled = HyperNodeService.Instance.EnableDiagnostics,
                TaskProgressCacheDuration = HyperNodeService.Instance.TaskProgressCacheDuration,
                TaskProgressCacheEnabled = HyperNodeService.Instance.EnableTaskProgressCache,
                MaxConcurrentTasks = HyperNodeService.Instance.MaxConcurrentTasks,
                Commands = HyperNodeService.Instance.GetCommandStatuses().ToList(),
                ActivityMonitors = HyperNodeService.Instance.GetActivityMonitorStatuses().ToList(),
                LiveTasks = HyperNodeService.Instance.GetLiveTaskStatuses().ToList(),
                ProcessStatusFlags = MessageProcessStatusFlags.Success
            };
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<GetNodeStatusResponse>();
        }
    }
}
