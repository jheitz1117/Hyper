using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCachedTaskProgressInfoCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            HyperNodeTaskProgressInfo response;

            var request = context.Request as GetCachedTaskProgressInfoRequest;
            if (request != null)
            {
                context.Activity.TrackFormat("Retrieving cached task progress info for Message Guid '{0}', Task ID '{1}'.", request.MessageGuid, request.TaskId);
                response = HyperNodeService.Instance.GetCachedTaskProgressInfo(request.MessageGuid, request.TaskId);
                response.ProcessStatusFlags = MessageProcessStatusFlags.Success;
            }
            else
            {
                context.Activity.Track("Request type '{0}' could not be converted to type '{1}'.", context.Request.GetType().FullName, typeof(GetCachedTaskProgressInfoRequest).FullName);
                //throw new InvalidRequestTypeException();
                throw new InvalidOperationException();
            }

            return response;
        }
    }
}
