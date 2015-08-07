using System;
using System.Collections.Generic;
using System.Threading;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public interface ICommandExecutionContext
    {
        string TaskId { get; }
        Guid MessageGuid { get; }
        string ExecutingNodeName { get; }
        string CommandName { get; }
        string CreatedByAgentName { get; }
        DateTime CreationDateTime { get; }
        IReadOnlyList<string> IntendedRecipientNodeNames { get; }
        IReadOnlyList<string> SeenByNodeNames { get; }
        MessageProcessOptionFlags ProcessOptionFlags { get; }
        ICommandRequest Request { get; }
        ITaskActivityTracker Activity { get; }
        CancellationToken Token { get; }
    }
}
