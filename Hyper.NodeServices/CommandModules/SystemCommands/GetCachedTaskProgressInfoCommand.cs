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
            var request = context.Request as GetCachedTaskProgressInfoRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(GetCachedTaskProgressInfoRequest), context.Request.GetType());

            context.Activity.TrackFormat("Retrieving cached task progress info for Message Guid '{0}', Task ID '{1}'.", request.MessageGuid, request.TaskId);
            var response = HyperNodeService.Instance.GetCachedTaskProgressInfo(request.MessageGuid, request.TaskId);
            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }
    }
}
