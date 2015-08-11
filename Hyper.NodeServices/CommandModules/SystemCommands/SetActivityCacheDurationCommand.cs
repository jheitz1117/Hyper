using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class SetActivityCacheDurationCommand : ICommandModule, ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
    {
        private readonly object _serializer;

        public SetActivityCacheDurationCommand()
        {
            _serializer = new NetDataContractCommandSerializer<SetActivityCacheDurationRequest, SetActivityCacheDurationResponse>();
        }

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as SetActivityCacheDurationRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(SetActivityCacheDurationRequest), context.Request.GetType());

            var response = new SetActivityCacheDurationResponse
            {
                ActivityCacheIsEnabled = HyperNodeService.Instance.EnableActivityCache
            };

            if (!response.ActivityCacheIsEnabled)
            {
                context.Activity.Track("Warning: The activity cache is disabled.");
                response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;
            }

            HyperNodeService.Instance.ActivityCacheSlidingExpiration = request.CacheDuration;
            context.Activity.TrackFormat("The activity cache duration is now {0}.", request.CacheDuration);

            response.ProcessStatusFlags |= MessageProcessStatusFlags.Success;

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
