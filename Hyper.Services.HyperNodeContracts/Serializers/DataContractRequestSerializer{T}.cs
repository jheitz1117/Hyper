using System.Runtime.Serialization;
using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public sealed class DataContractRequestSerializer<T> : XmlObjectRequestSerializer<T> where T : ICommandRequest
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(T));
        }
    }
}
