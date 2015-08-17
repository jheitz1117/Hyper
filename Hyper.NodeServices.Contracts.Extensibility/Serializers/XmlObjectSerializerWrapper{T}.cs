using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Hyper.Extensibility.IO;
using Hyper.IO;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Wraps an instance of <see cref="XmlObjectSerializer"/> and simplifies the task of serializing to and deserializing from string data.
    /// </summary>
    /// <typeparam name="T">The type of object to be serialized or deserialized.</typeparam>
    public abstract class XmlObjectSerializerWrapper<T>
    {
        private XmlObjectSerializer _serializer;
        private XmlObjectSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = CreateSerializer())); }
        }

        /// <summary>
        /// The <see cref="IStringTransform"/> to use when transforming the serialized bytes into a string.
        /// </summary>
        public IStringTransform SerializationTransform { get; set; }

        /// <summary>
        /// The <see cref="IStringTransform"/> to use when transforming a string into bytes to be deserialized.
        /// </summary>
        public IStringTransform DeserializationTransform { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper{T}"/> using <see cref="IStringTransform"/>
        /// instances created from <see cref="Encoding"/>.<see cref="Encoding.Default"/>.
        /// </summary>
        protected XmlObjectSerializerWrapper()
            : this(StringTransform.FromEncoding(Encoding.Default), StringTransform.FromEncoding(Encoding.Default)) { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper{T}"/> using the specified <see cref="IStringTransform"/>
        /// instances for serialization and deserialization, respectively.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming the serialized bytes into a string.</param>
        /// <param name="deserializationTransform">The <see cref="IStringTransform"/> to use when transforming a string into bytes to be deserialized.</param>
        protected XmlObjectSerializerWrapper(IStringTransform serializationTransform, IStringTransform deserializationTransform)
        {
            this.SerializationTransform = serializationTransform;
            this.DeserializationTransform = deserializationTransform;
        }

        /// <summary>
        /// Serializes the specified object into a string.
        /// </summary>
        /// <param name="target">The <typeparamref name="T"/> instance to serialize.</param>
        /// <returns></returns>
        protected string Serialize(T target)
        {
            using (var memory = new MemoryStream())
            {
                this.Serializer.WriteObject(memory, target);
                memory.Flush();

                return this.SerializationTransform.GetString(memory.ToArray());
            }
        }

        /// <summary>
        /// Deserializes the specified <paramref name="inputString"/> into an object of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="inputString">The string to deserialize.</param>
        /// <returns></returns>
        protected T Deserialize(string inputString)
        {
            using (var memory = new MemoryStream(this.DeserializationTransform.GetBytes(inputString)))
            {
                return (T)this.Serializer.ReadObject(memory);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates an instance of <see cref="XmlObjectSerializer"/> to use for serialization.
        /// </summary>
        /// <returns></returns>
        public abstract XmlObjectSerializer CreateSerializer();
    }
}
