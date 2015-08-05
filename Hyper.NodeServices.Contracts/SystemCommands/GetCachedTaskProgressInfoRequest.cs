using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetCachedTaskProgressInfoRequest : ICommandRequest
    {
        [DataMember]
        public Guid MessageGuid { get; set; }

        [DataMember]
        public string TaskId { get; set; }
    }
}
