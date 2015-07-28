using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleRequest
    {
        [DataMember]
        string TaskId { get; set; }

        [DataMember]
        Guid MessageGuid { get; set; }

        [DataMember]
        string CreatedByAgentName { get; set; }

        [DataMember]
        DateTime CreationDateTime { get; set; }

        [DataMember]
        List<string> IntendedRecipientNodeNames { get; set; }

        [DataMember]
        List<string> SeenByNodeNames { get; set; }
    }
}
