using System.Runtime.Serialization;

namespace Hyper.Services.HyperNodeContracts
{
    [DataContract]
    public class HyperNodeVertex : IVertex
    {
        [DataMember]
        public string Key { get; set; }
    }
}
