﻿using System;
using System.Collections.Generic;
using System.Threading;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility
{
    public interface ICommandExecutionContext
    {
        string TaskId { get; }
        Guid MessageGuid { get; }
        string CommandName { get; }
        string CreatedByAgentName { get; }
        DateTime CreationDateTime { get; }
        List<string> IntendedRecipientNodeNames { get; }
        List<string> SeenByNodeNames { get; }
        ICommandRequest Request { get; }
        ITaskActivityTracker Activity { get; }
        CancellationToken Token { get; }
    }
}