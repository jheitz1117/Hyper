using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetChildNodesResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public List<string> ChildNodes { get; set; }

        public GetChildNodesResponse()
        {
            this.ChildNodes = new List<string>();
        }
    }
}
