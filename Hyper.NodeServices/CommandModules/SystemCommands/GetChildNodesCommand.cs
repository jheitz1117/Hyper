﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class GetChildNodesCommand : ICommandModule, ICommandResponseSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var response = new GetChildNodesResponse();

            context.Activity.Track("Retrieving child nodes.");

            response.ChildNodes.AddRange(
                HyperNodeService.Instance.GetChildNodes()
            );

            response.ProcessStatusFlags = MessageProcessStatusFlags.Success;

            return response;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return new NetDataContractResponseSerializer<GetChildNodesResponse>();
        }
    }
}
