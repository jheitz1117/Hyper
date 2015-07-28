using System;
using System.Collections.Generic;

namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class CommandRequestString : ICommandModuleRequest
    {
        public string RequestString { get; set; }
        public string TaskId { get; set; }
        public Guid MessageGuid { get; set; }
        public string CreatedByAgentName { get; set; }
        public DateTime CreationDateTime { get; set; }
        public List<string> IntendedRecipientNodeNames { get; set; }
        public List<string> SeenByNodeNames { get; set; }

        public CommandRequestString(string requestString)
        {
            this.RequestString = requestString;
        }

        public override string ToString()
        {
            return this.RequestString;
        }
    }
}
