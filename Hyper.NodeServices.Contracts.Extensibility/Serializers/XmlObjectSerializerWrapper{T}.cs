using Hyper.Extensibility.IO;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Abstract generic version of <see cref="XmlObjectSerializerWrapper"/>
    /// </summary>
    /// <typeparam name="T">The type of object to be serialized or deserialized.</typeparam>
    public abstract class XmlObjectSerializerWrapper<T> : XmlObjectSerializerWrapper
    {
        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper{T}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>.
        /// </summary>
        protected XmlObjectSerializerWrapper() { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper{T}"/> using the specified <see cref="IStringTransform"/> instance.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.</param>
        protected XmlObjectSerializerWrapper(IStringTransform serializationTransform)
            : base(serializationTransform) { }

        /// <summary>
        /// Serializes the specified object into a string.
        /// </summary>
        /// <param name="target">The <typeparamref name="T"/> instance to serialize.</param>
        /// <returns></returns>
        protected string Serialize(T target)
        {
            return base.Serialize(target);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="inputString"/> into an object of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="inputString">The string to deserialize.</param>
        /// <returns></returns>
        protected new T Deserialize(string inputString)
        {
            return (T) base.Deserialize(inputString);
        }
    }
}
