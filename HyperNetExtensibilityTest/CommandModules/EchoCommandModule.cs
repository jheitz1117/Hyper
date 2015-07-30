﻿using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeContracts.Extensibility;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class EchoCommandModule : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = (CommandRequestString) context.Request;
            context.Activity.Track(request.RequestString);
            
            return new CommandResponseString(MessageProcessStatusFlags.Success, request.RequestString);
        }
    }
}
