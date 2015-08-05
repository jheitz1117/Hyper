using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetKnownCommandsCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new GetKnownCommandsResponse();

            context.Activity.Track("Retrieving known commands.");
            
            response.KnownCommands.AddRange(
                HyperNodeService.Instance.GetKnownCommands()
            );
            
            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }
    }
}
