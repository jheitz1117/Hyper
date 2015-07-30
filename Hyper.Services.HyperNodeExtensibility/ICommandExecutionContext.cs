using System;
using System.Collections.Generic;
using System.Threading;
using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandExecutionContext
    {
        string TaskId { get; }
        Guid MessageGuid { get; }
        string CreatedByAgentName { get; }
        DateTime CreationDateTime { get; }
        List<string> IntendedRecipientNodeNames { get; }
        List<string> SeenByNodeNames { get; }
        ICommandRequest Request { get; }
        ITaskActivityTracker Activity { get; }
        CancellationToken Token { get; }
    }
}
