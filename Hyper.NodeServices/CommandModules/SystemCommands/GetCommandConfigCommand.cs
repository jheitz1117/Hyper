using System.Linq;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCommandConfigCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new GetCommandConfigResponse();

            context.Activity.Track("Retrieving known commands.");

            response.CommandConfigurations = HyperNodeService.Instance.GetCommandConfig().ToList();
            
            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<GetCommandConfigResponse>();
        }
    }
}
