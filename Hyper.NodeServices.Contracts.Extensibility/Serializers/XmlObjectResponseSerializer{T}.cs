using System;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Abstract implementation of <see cref="XmlObjectSerializerWrapper{T}"/> that can serialize instances of <see cref="ICommandResponse"/>.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public abstract class XmlObjectResponseSerializer<T> : XmlObjectSerializerWrapper<T>, ICommandResponseSerializer
        where T : ICommandResponse
    {
        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectResponseSerializer{T}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>.
        /// </summary>
        protected XmlObjectResponseSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectResponseSerializer{T}"/> using the specified <see cref="IStringTransform"/> instance.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.</param>
        protected XmlObjectResponseSerializer(IStringTransform serializationTransform)
            : base(serializationTransform) { }
        

        /// <summary>
        /// Serializes the specified <see cref="ICommandResponse"/> into a string.
        /// </summary>
        /// <param name="response">The <typeparamref name="T"/> instance to serialize. This instance must implement <see cref="ICommandResponse"/>.</param>
        /// <returns></returns>
        public string Serialize(ICommandResponse response)
        {
            return base.Serialize((T)response);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="responseString"/> into an object of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="responseString">The string to deserialize.</param>
        /// <returns></returns>
        ICommandResponse ICommandResponseSerializer.Deserialize(string responseString)
        {
            return base.Deserialize(responseString);
        }

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandResponse"/> object.
        /// </summary>
        /// <returns></returns>
        public Type GetResponseType()
        {
            return typeof(T);
        }
    }
}
