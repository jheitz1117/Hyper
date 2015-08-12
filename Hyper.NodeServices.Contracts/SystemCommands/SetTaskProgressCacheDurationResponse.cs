using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class SetTaskProgressCacheDurationResponse : ICommandResponse
    {
        [DataMember]
        public bool TaskProgressCacheEnabled { get; set; }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
