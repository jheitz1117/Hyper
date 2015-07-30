using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class EchoCommandModule : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = (CommandRequestString) context.Request;
            context.Activity.Track(request.RequestString);
            
            return new CommandResponseString(MessageProcessStatusFlags.Success, request.RequestString);
        }
    }
}
