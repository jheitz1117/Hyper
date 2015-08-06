using System.Linq;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCommandConfigCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new GetCommandConfigResponse();

            context.Activity.Track("Retrieving known commands.");

            response.CommandConfigurations = HyperNodeService.Instance.GetCommandConfig().ToList();
            
            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }
    }
}
