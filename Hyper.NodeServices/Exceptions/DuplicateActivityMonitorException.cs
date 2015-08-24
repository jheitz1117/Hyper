using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices
{
    [Serializable]
    internal class DuplicateActivityMonitorException : Exception
    {
        public DuplicateActivityMonitorException() { }
        public DuplicateActivityMonitorException(string message) : base(message) { }
        public DuplicateActivityMonitorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public DuplicateActivityMonitorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
