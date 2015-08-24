using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.TaskIdProviders
{
    /// <summary>
    /// This class provides a read-only version of a <see cref="HyperNodeMessageRequest"/> for use in user-defined implementations of the <see cref="ITaskIdProvider"/> interface.
    /// </summary>
    internal class TaskIdCreationContext : IHyperNodeMessageContext
    {
        public string CommandName { get; set; }
        public string CreatedByAgentName { get; set; }
        public DateTime CreationDateTime { get; set; }
        public IReadOnlyList<string> IntendedRecipientNodeNames { get; set; }
        public IReadOnlyList<string> SeenByNodeNames { get; set; }
        public MessageProcessOptionFlags ProcessOptionFlags { get; set; }

        public TaskIdCreationContext(IList<string> intendedRecipientNodeNames, IList<string> seenByNodeNames)
        {
            // Copy in the info from our top-level message and response. We're avoiding assignment so that
            // if the user changes anything, it doesn't affect the top-level message
            this.IntendedRecipientNodeNames = new ReadOnlyCollection<string>(intendedRecipientNodeNames);
            this.SeenByNodeNames = new ReadOnlyCollection<string>(seenByNodeNames);
        }
    }
}
