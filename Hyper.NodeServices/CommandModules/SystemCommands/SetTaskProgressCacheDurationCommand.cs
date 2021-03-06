﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class SetTaskProgressCacheDurationCommand : ICommandModule, ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
    {
        private readonly object _serializer;

        public SetTaskProgressCacheDurationCommand()
        {
            _serializer = new NetDataContractCommandSerializer<SetTaskProgressCacheDurationRequest, SetTaskProgressCacheDurationResponse>();
        }

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as SetTaskProgressCacheDurationRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(SetTaskProgressCacheDurationRequest), context.Request.GetType());

            var response = new SetTaskProgressCacheDurationResponse
            {
                TaskProgressCacheEnabled = HyperNodeService.Instance.EnableTaskProgressCache
            };

            if (!response.TaskProgressCacheEnabled)
            {
                context.Activity.Track("Warning: The task progress cache is disabled.");
                response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;
            }

            HyperNodeService.Instance.TaskProgressCacheDuration = request.CacheDuration;
            context.Activity.Track($"The task progress cache duration is now {request.CacheDuration}.");
            
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
