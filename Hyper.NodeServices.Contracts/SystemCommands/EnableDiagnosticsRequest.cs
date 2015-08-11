using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class EnableDiagnosticsRequest : ICommandRequest
    {
        [DataMember]
        public bool Enable { get; set; }
    }
}
