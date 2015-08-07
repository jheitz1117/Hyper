using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetChildNodesCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new GetChildNodesResponse();

            context.Activity.Track("Retrieving child nodes.");

            response.ChildNodes.AddRange(
                HyperNodeService.Instance.GetChildNodes()
            );

            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<GetChildNodesResponse>();
        }
    }
}
