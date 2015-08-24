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
    public abstract class XmlObjectSerializerWrapper
    {
        /// <summary>
        /// The default implementation of <see cref="IStringTransform"/> to use for serialization and deserialization is
        /// created from <see cref="Encoding.Default"/>.
        /// </summary>
        public static readonly IStringTransform DefaultStringTransform = StringTransform.FromEncoding(Encoding.Default);
        
        private XmlObjectSerializer _serializer;
        private XmlObjectSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = CreateSerializer())); }
        }

        /// <summary>
        /// The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.
        /// </summary>
        public IStringTransform SerializationTransform { get; private set; }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>.
        /// </summary>
        protected XmlObjectSerializerWrapper()
            : this(DefaultStringTransform) { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectSerializerWrapper"/> using the specified <see cref="IStringTransform"/> instance.
        /// </summary>
        /// <param name="serializationTransform">The <see cref="IStringTransform"/> to use when transforming data between string and byte representations.</param>
        protected XmlObjectSerializerWrapper(IStringTransform serializationTransform)
        {
            this.SerializationTransform = serializationTransform;
        }

        /// <summary>
        /// Serializes the specified object into a string.
        /// </summary>
        /// <returns></returns>
        protected string Serialize(object target)
        {
            using (var memory = new MemoryStream())
            {
                this.Serializer.WriteObject(memory, target);
                memory.Flush();

                return this.SerializationTransform.GetString(memory.ToArray());
            }
        }

        /// <summary>
        /// Deserializes the specified <paramref name="inputString"/> into an object.
        /// </summary>
        /// <param name="inputString">The string to deserialize.</param>
        /// <returns></returns>
        protected object Deserialize(string inputString)
        {
            using (var memory = new MemoryStream(this.SerializationTransform.GetBytes(inputString)))
            {
                return this.Serializer.ReadObject(memory);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates an instance of <see cref="XmlObjectSerializer"/> to use for serialization.
        /// </summary>
        /// <returns></returns>
        public abstract XmlObjectSerializer CreateSerializer();
    }
}
