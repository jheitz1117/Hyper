using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetCachedTaskProgressInfoResponse : ICommandResponse
    {
        [DataMember]
        public bool ActivityCacheIsEnabled { get; set; }

        [DataMember]
        public HyperNodeTaskProgressInfo TaskProgressInfo { get; set; }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
