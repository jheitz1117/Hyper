using System;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility
{
    public interface IHyperNodeMessageContext
    {
        Guid MessageGuid { get; }
        string CommandName { get; }
        string CreatedByAgentName { get; }
        DateTime CreationDateTime { get; }
        IReadOnlyList<string> IntendedRecipientNodeNames { get; }
        IReadOnlyList<string> SeenByNodeNames { get; }
        MessageProcessOptionFlags ProcessOptionFlags { get; }
    }
}
