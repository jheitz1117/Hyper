using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetCommandConfigResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public IEnumerable<CommandConfiguration> CommandConfigurations { get; set; }

        public GetCommandConfigResponse()
        {
            this.CommandConfigurations = new List<CommandConfiguration>();
        }
    }
}
