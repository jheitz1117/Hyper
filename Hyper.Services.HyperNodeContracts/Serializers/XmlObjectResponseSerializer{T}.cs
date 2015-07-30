using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public abstract class XmlObjectResponseSerializer<T> : XmlObjectSerializerWrapper<T>, ICommandResponseSerializer
        where T : ICommandResponse
    {
        public string Serialize(ICommandResponse request)
        {
            return base.Serialize((T)request);
        }

        ICommandResponse ICommandResponseSerializer.Deserialize(string requestString)
        {
            return base.Deserialize(requestString);
        }
    }
}
