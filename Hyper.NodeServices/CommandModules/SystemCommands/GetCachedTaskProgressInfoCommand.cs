using System;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetCachedTaskProgressInfoCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as CommandRequestString;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(CommandRequestString), context.Request.GetType());

            var response = new GetCachedTaskProgressInfoResponse
            {
                TaskProgressCacheEnabled = HyperNodeService.Instance.EnableTaskProgressCache
            };

            if (!response.TaskProgressCacheEnabled)
            {
                context.Activity.Track("Warning: The task progress cache is disabled.");
                response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;
            }

            context.Activity.Track($"Retrieving cached task progress info for Task ID '{request.RequestString}'.");
            response.TaskProgressInfo = HyperNodeService.Instance.GetCachedTaskProgressInfo(request.RequestString);

            // If we can't find any task progress info for the specified Task ID, we'll return a placeholder object in Completed status that informs the caller that no progress
            // information exists for this task ID. This will prevent the caller from sitting in an infinite loop waiting for IsComplete to be true when there may not be a cache
            // item available, or no cache at all
            response.TaskProgressInfo = response.TaskProgressInfo ?? new HyperNodeTaskProgressInfo
            {
                IsComplete = true,
                Activity = new List<HyperNodeActivityItem>
                {
                    new HyperNodeActivityItem
                    {
                        EventDateTime = DateTime.Now,
                        EventDescription = $"No task progress information exists for Task ID '{request.RequestString}'.",
                        Agent = context.ExecutingNodeName
                    }
                }
            };

            response.ProcessStatusFlags |= MessageProcessStatusFlags.Success;

            return response;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<GetCachedTaskProgressInfoResponse>();
        }
    }
}
