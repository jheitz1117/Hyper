using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCachedProgressInfoCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            HyperNodeProgressInfo response;

            var commandRequest = context.Request as CommandRequestString;
            if (commandRequest != null)
            {
                context.Activity.TrackFormat("Retrieving cached progress info for Message Guid '{0}'.", context.MessageGuid);
                response = HyperNodeService.Instance.GetCachedProgressInfo(commandRequest.RequestString);
                response.ProcessStatusFlags = MessageProcessStatusFlags.Success;
            }
            else
            {
                context.Activity.Track("Request type '{0}' could not be converted to type '{1}'.", context.Request.GetType().FullName, typeof(CommandRequestString).FullName);
                //throw new InvalidRequestTypeException();
                throw new InvalidOperationException();
            }

            return response;
        }
    }
}
