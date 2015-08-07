using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCachedTaskProgressInfoCommand : ICommandModule, ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
    {
        private readonly object _serializer;

        public GetCachedTaskProgressInfoCommand()
        {
            _serializer = new NetDataContractCommandSerializer<GetCachedTaskProgressInfoRequest, HyperNodeTaskProgressInfo>();
        }

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

        ICommandRequestSerializer ICommandRequestSerializerFactory.Create()
        {
            return _serializer as ICommandRequestSerializer;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return _serializer as ICommandResponseSerializer;
        }
    }
}
