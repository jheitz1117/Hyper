using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class DiscoverResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public ConcurrentDictionary<string, DiscoverResponse> ChildNodes { get; set; }

        public DiscoverResponse()
        {
            this.ChildNodes = new ConcurrentDictionary<string, DiscoverResponse>();
        }
    }
}
