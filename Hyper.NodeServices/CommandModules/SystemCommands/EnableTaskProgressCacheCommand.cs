using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class EnableTaskProgressCacheCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as EnableTaskProgressCacheRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(EnableTaskProgressCacheRequest), context.Request.GetType());

            HyperNodeService.Instance.EnableTaskProgressCache = request.Enable;
            context.Activity.TrackFormat(
                "The task progress cache is now {0}.",
                (request.Enable ? "enabled" : "disabled")
            );

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableTaskProgressCacheRequest>();
        }
    }
}
