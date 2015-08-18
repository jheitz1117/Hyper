using System;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Abstract implementation of <see cref="XmlObjectSerializerWrapper{T}"/> that can serialize instances of <see cref="ICommandRequest"/>.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    public abstract class XmlObjectRequestSerializer<T> : XmlObjectSerializerWrapper<T>, ICommandRequestSerializer
        where T : ICommandRequest
    {
        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectRequestSerializer{T}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>.
        /// </summary>
        protected XmlObjectRequestSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectRequestSerializer{T}"/> using the specified <see cref="IStringTransform"/> instance.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.</param>
        protected XmlObjectRequestSerializer(IStringTransform serializationTransform)
            : base(serializationTransform) { }

        /// <summary>
        /// Serializes the specified <see cref="ICommandRequest"/> into a string.
        /// </summary>
        /// <param name="request">The <typeparamref name="T"/> instance to serialize. This instance must implement <see cref="ICommandRequest"/>.</param>
        /// <returns></returns>
        public string Serialize(ICommandRequest request)
        {
            return base.Serialize((T)request);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="requestString"/> into an object of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="requestString">The string to deserialize.</param>
        /// <returns></returns>
        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return base.Deserialize(requestString);
        }

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandRequest"/> object.
        /// </summary>
        /// <returns></returns>
        public Type GetRequestType()
        {
            return typeof(T);
        }
    }
}
