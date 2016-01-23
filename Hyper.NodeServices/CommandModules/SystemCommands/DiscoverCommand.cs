using System;
using System.Linq;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class DiscoverCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new DiscoverResponse();

            context.Activity.Track("Discovering HyperNode network.");

            var childNodeNames = HyperNodeService.Instance.GetChildNodes().ToList();

            if (childNodeNames.Any())
            {
                // Note that we're deliberately NOT running this discover request concurrently. This would defeat the purpose.
                var discoverRequest = new HyperNodeMessageRequest(context.ExecutingNodeName)
                {
                    CommandName = context.CommandName
                };

                // Enable caching if the caller originally requested it
                if ((context.ProcessOptionFlags & MessageProcessOptionFlags.CacheTaskProgress) == MessageProcessOptionFlags.CacheTaskProgress)
                    discoverRequest.ProcessOptionFlags |= MessageProcessOptionFlags.CacheTaskProgress;

                // Enable task trace if the caller originally requested it
                if ((context.ProcessOptionFlags & MessageProcessOptionFlags.ReturnTaskTrace) == MessageProcessOptionFlags.ReturnTaskTrace)
                    discoverRequest.ProcessOptionFlags |= MessageProcessOptionFlags.ReturnTaskTrace;

                // Record all the nodes that have already seen this discover request. This list will include the current node.
                discoverRequest.SeenByNodeNames.AddRange(context.SeenByNodeNames);

                // Create a serializer for ourselves
                var serializer = ((ICommandResponseSerializerFactory)this).Create();

                // Now send the request to all children
                foreach (var childNodeName in childNodeNames)
                {
                    DiscoverResponse childDiscoverResponse = null;

                    try
                    {
                        using (var client = new HyperNodeClient(childNodeName))
                        {
                            // Discover grandchildren
                            var childResponse = client.ProcessMessage(discoverRequest);

                            // Check if the message was accepted, and that the command was recognized
                            if (childResponse.NodeAction == HyperNodeActionType.Accepted)
                            {
                                context.Activity.Track($"Child node '{childNodeName}' accepted the request.");

                                // Check if the child node recognized the command. If not, skip to the next child
                                if ((childResponse.ProcessStatusFlags & MessageProcessStatusFlags.InvalidCommand) == MessageProcessStatusFlags.InvalidCommand)
                                {
                                    context.Activity.Track(
                                        $"Child node '{childNodeName}' did not recognize the command name '{discoverRequest.CommandName}'."
                                    );

                                    continue;
                                }

                                // Deserialize discover response
                                childDiscoverResponse = serializer.Deserialize(childResponse.CommandResponseString) as DiscoverResponse;

                                // Add in any extra status flags imposed by the child call
                                response.ProcessStatusFlags |= childResponse.ProcessStatusFlags;
                            }
                            else
                            {
                                context.Activity.Track(
                                    $"Child node '{childResponse.RespondingNodeName}' did not accept the request.",
                                    $"The {nameof(childResponse.NodeAction)} was '{childResponse.NodeAction}' and the {nameof(childResponse.NodeActionReason)} was '{childResponse.NodeActionReason}'."
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Activity.TrackException(ex);
                        response.ProcessStatusFlags |= MessageProcessStatusFlags.HadNonFatalErrors;
                    }
                    finally
                    {
                        // Add the new response to our response tree regardless
                        response.ChildNodes.TryAdd(childNodeName, childDiscoverResponse);
                    }
                }
            }
            else
            {
                context.Activity.Track("No child nodes found.");
            }

            response.ProcessStatusFlags |= MessageProcessStatusFlags.Success;

            return response;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<DiscoverResponse>();
        }
    }
}
