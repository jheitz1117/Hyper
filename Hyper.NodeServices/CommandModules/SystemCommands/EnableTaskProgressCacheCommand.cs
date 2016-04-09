using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

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
            context.Activity.Track($"The task progress cache is now {(request.Enable ? "enabled" : "disabled")}.");

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableTaskProgressCacheRequest>();
        }
    }
}
