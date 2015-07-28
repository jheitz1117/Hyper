using System;
using System.Collections.Generic;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNet.ExtensibilityTest.Shared.CommandModules
{
    public class ComplexCommandRequest : ICommandModuleRequest
    {
        public string TaskId { get; set; }

        public Guid MessageGuid { get; set; }

        public string CreatedByAgentName { get; set; }

        public DateTime CreationDateTime { get; set; }

        public List<string> IntendedRecipientNodeNames { get; set; }

        public List<string> SeenByNodeNames { get; set; }

        public string MyString { get; set; }
        public DateTime MyDateTime { get; set; }
        public int MyInt32 { get; set; }
        public TimeSpan MyTimeSpan { get; set; }
    }
}
