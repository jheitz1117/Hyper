using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class EchoCommandModule : SimpleCommandModule
    {
        public override ICommandModuleResponse Execute(ICommandExecutionContext context)
        {
            var request = (CommandRequestString) context.Request;
            context.Activity.Track(request.RequestString);
            
            return new CommandResponseString(MessageProcessStatusFlags.Success, request.RequestString);
        }
    }
}
