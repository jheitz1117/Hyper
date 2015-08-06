using System;
using System.Linq;
using Hyper.NodeServices.Client;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

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
                    CommandName = SystemCommandNames.Discover
                };

                // Enable caching if the caller originally requested it
                if ((context.ProcessOptionFlags & MessageProcessOptionFlags.CacheProgressInfo) == MessageProcessOptionFlags.CacheProgressInfo)
                    discoverRequest.ProcessOptionFlags |= MessageProcessOptionFlags.CacheProgressInfo;

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
                        // Discover grandchildren
                        var childResponse = new HyperNodeClient(childNodeName).ProcessMessage(discoverRequest);

                        // Check if the message was accepted, and that the command was recognized
                        if (childResponse.NodeAction == HyperNodeActionType.Accepted)
                        {
                            context.Activity.TrackFormat("Child node '{0}' accepted the request.", childNodeName);

                            // Check if the child node recognized the command. If not, skip to the next child
                            if ((childResponse.ProcessStatusFlags & MessageProcessStatusFlags.InvalidCommand) == MessageProcessStatusFlags.InvalidCommand)
                            {
                                context.Activity.TrackFormat(
                                    "Child node '{0}' did not recognize the command name '{1}'.",
                                    childNodeName,
                                    discoverRequest.CommandName
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
                                "Child node '{0}' did not accept the request.",
                                string.Format(
                                    "The node action was '{0}' and the action reason was '{1}'.",
                                    childResponse.NodeAction,
                                    childResponse.NodeActionReason
                                    )
                                );
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
