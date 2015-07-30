using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public sealed class DataContractResponseJsonSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractJsonSerializer(typeof(T));
        }
    }
}
