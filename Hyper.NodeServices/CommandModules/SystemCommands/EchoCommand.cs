using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class EchoCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as CommandRequestString;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(CommandRequestString), context.Request.GetType());

            var echoString = string.Format("HyperNode '{0}' says, \"{1}\".", context.ExecutingNodeName, request.RequestString);
            
            context.Activity.TrackFormat(echoString);

            return new CommandResponseString(MessageProcessStatusFlags.Success, echoString);
        }
    }
}
