using System;
using System.Runtime.Serialization;

namespace Hyper.Db.Xml
{
    [Serializable]
    public class HyperDbSchemaConfigurationException : Exception
    {
        public HyperDbSchemaConfigurationException() { }
        public HyperDbSchemaConfigurationException(string message) : base(message) { }
        public HyperDbSchemaConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public HyperDbSchemaConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
