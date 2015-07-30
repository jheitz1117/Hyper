﻿using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModule
    {
        ICommandResponse Execute(ICommandExecutionContext context);
    }
}
