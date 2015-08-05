using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetKnownCommandsResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public List<string> KnownCommands { get; set; }

        public GetKnownCommandsResponse()
        {
            this.KnownCommands = new List<string>();
        }
    }
}
