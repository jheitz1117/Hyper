using System.Runtime.Serialization;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectRequestSerializer{T}"/> that can serialize instances of <see cref="ICommandRequest"/>
    /// using a <see cref="DataContractSerializer"/>. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    public sealed class DataContractRequestSerializer<T> : XmlObjectRequestSerializer<T> where T : ICommandRequest
    {
        /// <summary>
        /// Initializes an instance of <see cref="DataContractRequestSerializer{T}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>.
        /// </summary>
        public DataContractRequestSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="DataContractRequestSerializer{T}"/> using the specified <see cref="IStringTransform"/> instance.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.</param>
        public DataContractRequestSerializer(IStringTransform serializationTransform)
            : base(serializationTransform) { }

        /// <summary>
        /// Creates an instance of <see cref="DataContractSerializer"/> to use for serialization.
        /// </summary>
        /// <returns></returns>
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(T));
        }
    }
}
