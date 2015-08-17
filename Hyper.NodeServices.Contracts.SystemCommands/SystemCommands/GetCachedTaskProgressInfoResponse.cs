using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetCachedTaskProgressInfoResponse : ICommandResponse
    {
        [DataMember]
        public bool TaskProgressCacheEnabled { get; set; }

        [DataMember]
        public HyperNodeTaskProgressInfo TaskProgressInfo { get; set; }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
