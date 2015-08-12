using System;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

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
                ActivityCacheIsEnabled = HyperNodeService.Instance.EnableActivityCache
            };

            if (!response.ActivityCacheIsEnabled)
            {
                context.Activity.Track("Warning: The activity cache is disabled.");
                response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;
            }

            context.Activity.TrackFormat("Retrieving cached task progress info for Task ID '{0}'.", request.RequestString);
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
                        EventDescription = string.Format("No task progress information exists for Task ID '{0}'.", request.RequestString),
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
