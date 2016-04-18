using System;
using System.Runtime.Serialization;

namespace Hyper.Db.Xml
{
    [Serializable]
    public class XmlDbSchemaException : Exception
    {
        public XmlDbSchemaException() { }
        public XmlDbSchemaException(string message) : base(message) { }
        public XmlDbSchemaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public XmlDbSchemaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
