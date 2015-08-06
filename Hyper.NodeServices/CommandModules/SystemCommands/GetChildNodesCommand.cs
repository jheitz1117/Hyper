using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    public class GetChildNodesCommand : ICommandModule
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
    }
}
