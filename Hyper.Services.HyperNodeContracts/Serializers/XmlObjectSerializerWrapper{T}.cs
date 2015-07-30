using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public abstract class XmlObjectSerializerWrapper<T>
    {
        private XmlObjectSerializer _serializer;
        private XmlObjectSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = CreateSerializer())); }
        }

        public Encoding SerializeToEncoding { get; set; }
        public Encoding DeserializeFromEncoding { get; set; }

        protected XmlObjectSerializerWrapper()
            : this(Encoding.Default, Encoding.Default) { }

        protected XmlObjectSerializerWrapper(Encoding serializeToEncoding, Encoding deserializeFromEncoding)
        {
            this.SerializeToEncoding = serializeToEncoding;
            this.DeserializeFromEncoding = deserializeFromEncoding;
        }

        protected string Serialize(T target)
        {
            using (var memory = new MemoryStream())
            {
                this.Serializer.WriteObject(memory, target);
                memory.Flush();

                return this.SerializeToEncoding.GetString(memory.ToArray());
            }
        }

        protected T Deserialize(string inputString)
        {
            using (var memory = new MemoryStream(this.DeserializeFromEncoding.GetBytes(inputString)))
            {
                return (T)this.Serializer.ReadObject(memory);
            }
        }

        public abstract XmlObjectSerializer CreateSerializer();
    }
}
