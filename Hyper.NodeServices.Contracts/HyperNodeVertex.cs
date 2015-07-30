using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    [DataContract]
    public class HyperNodeVertex : IVertex
    {
        [DataMember]
        public string Key { get; set; }
    }
}
