using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices
{
    [Serializable]
    internal class HyperNodeConfigurationException : ApplicationException
    {
        public HyperNodeConfigurationException() { }
        public HyperNodeConfigurationException(string message) : base(message) { }
        public HyperNodeConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public HyperNodeConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
