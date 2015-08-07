using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class EnableActivityMonitorRequest : ICommandRequest
    {
        [DataMember]
        public string ActivityMonitorName { get; set; }

        [DataMember]
        public bool Enable { get; set; }
    }
}
