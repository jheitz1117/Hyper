using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class DataContractRequestJsonSerializer<T> : XmlObjectRequestSerializer<T> where T : ICommandRequest
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractJsonSerializer(typeof(T));
        }
    }
}
