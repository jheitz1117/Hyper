using System;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
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

        public virtual Type GetResponseType()
        {
            return typeof(T);
        }
    }
}
