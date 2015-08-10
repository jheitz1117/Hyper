using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class EnableActivityCacheRequest : ICommandRequest
    {
        [DataMember]
        public bool Enable { get; set; }
    }
}
