using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public abstract class XmlObjectSerializerWrapper<T>
    {
        private XmlObjectSerializer _serializer;
        private XmlObjectSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = CreateSerializer())); }
        }

        protected string Serialize(T target)
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    this.Serializer.WriteObject(writer, target);
                    writer.Flush();
                }
            }

            return builder.ToString();
        }

        protected T Deserialize(string inputString)
        {
            using (var stringReader = new StringReader(inputString))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)this.Serializer.ReadObject(xmlReader);
                }
            }
        }

        public abstract XmlObjectSerializer CreateSerializer();
    }
}
