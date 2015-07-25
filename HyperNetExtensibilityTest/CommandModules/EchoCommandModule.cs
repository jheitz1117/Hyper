using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class EchoCommandModule : ICommandModule
    {
        public ICommandModuleResponse Execute(ICommandExecutionContext context)
        {
            var request = (CommandRequestString) context.Request;
            context.Activity.Track(request.RequestString);
            
            return new CommandResponseString(request.RequestString, MessageProcessStatusFlags.Success);
        }
    }
}
