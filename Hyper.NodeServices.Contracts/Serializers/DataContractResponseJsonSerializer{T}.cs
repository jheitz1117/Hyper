using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectResponseSerializer{T}"/> that can serialize instances of <see cref="ICommandResponse"/>
    /// using a <see cref="DataContractJsonSerializer"/>. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public sealed class DataContractResponseJsonSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        /// <summary>
        /// Creates an instance of <see cref="DataContractJsonSerializer"/> to use for serialization.
        /// </summary>
        /// <returns></returns>
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractJsonSerializer(typeof(T));
        }
    }
}
