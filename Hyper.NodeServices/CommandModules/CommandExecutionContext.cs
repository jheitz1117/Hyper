using System;
using System.Collections.Generic;
using System.Threading;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.CommandModules
{
    internal class CommandExecutionContext : ICommandExecutionContext
    {
        public string TaskId { get; set; }
        public Guid MessageGuid { get; set; }
        public string ExecutingNodeName { get; set; }
        public string CommandName { get; set; }
        public string CreatedByAgentName { get; set; }
        public DateTime CreationDateTime { get; set; }
        public IReadOnlyList<string> IntendedRecipientNodeNames { get; set; }
        public IReadOnlyList<string> SeenByNodeNames { get; set; }
        public MessageProcessOptionFlags ProcessOptionFlags { get; set; }
        public ICommandRequest Request { get; set; }
        public ITaskActivityTracker Activity { get; set; }
        public CancellationToken Token { get; set; }

        public CommandExecutionContext(IEnumerable<string> intendedRecipientNodeNames, IEnumerable<string> seenByNodeNames)
        {
            // Copy in the info from our top-level message and response. We're avoiding assignment so that
            // if the user changes anything, it doesn't affect the top-level message
            this.IntendedRecipientNodeNames = new List<string>(intendedRecipientNodeNames);
            this.SeenByNodeNames = new List<string>(seenByNodeNames);
        }
    }
}
