using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices
{
    /// <summary>
    /// This class provides a read-only version of a <see cref="HyperNodeMessageRequest"/> for use in user-defined code.
    /// </summary>
    internal class ReadOnlyHyperNodeMessageInfo : IReadOnlyHyperNodeMessageInfo
    {
        public string CommandName { get; }
        public string CreatedByAgentName { get; }
        public DateTime CreationDateTime { get; }
        public IReadOnlyList<string> IntendedRecipientNodeNames { get; }
        public IReadOnlyList<string> SeenByNodeNames { get; }
        public MessageProcessOptionFlags ProcessOptionFlags { get; }

        public ReadOnlyHyperNodeMessageInfo(HyperNodeMessageRequest message)
        {
            CommandName = message.CommandName;
            CreatedByAgentName = message.CreatedByAgentName;
            CreationDateTime = message.CreationDateTime;
            ProcessOptionFlags = message.ProcessOptionFlags;
            IntendedRecipientNodeNames = new ReadOnlyCollection<string>(message.IntendedRecipientNodeNames);
            SeenByNodeNames = new ReadOnlyCollection<string>(message.SeenByNodeNames);
        }
    }
}
