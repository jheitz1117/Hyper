using System;
using System.Collections.Generic;
using System.Threading;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules
{
    internal class CommandExecutionContext : ICommandExecutionContext
    {
        public string TaskId { get; set; }
        public Guid MessageGuid { get; set; }
        public string CreatedByAgentName { get; set; }
        public DateTime CreationDateTime { get; set; }
        public List<string> IntendedRecipientNodeNames { get; set; }
        public List<string> SeenByNodeNames { get; set; }
        public ICommandRequest Request { get; set; }
        public ITaskActivityTracker Activity { get; set; }
        public CancellationToken Token { get; set; }

        public CommandExecutionContext()
        {
            this.IntendedRecipientNodeNames = new List<string>();
            this.SeenByNodeNames = new List<string>();
        }
    }
}
