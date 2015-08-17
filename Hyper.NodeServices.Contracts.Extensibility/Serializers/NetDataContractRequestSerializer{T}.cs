using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectRequestSerializer{T}"/> that can serialize instances of <see cref="ICommandRequest"/>
    /// using a <see cref="NetDataContractSerializer"/>. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    public sealed class NetDataContractRequestSerializer<T> : XmlObjectRequestSerializer<T> where T : ICommandRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="NetDataContractSerializer"/> to use for serialization.
        /// </summary>
        /// <returns></returns>
        public override XmlObjectSerializer CreateSerializer()
        {
            return new NetDataContractSerializer();
        }
    }
}
