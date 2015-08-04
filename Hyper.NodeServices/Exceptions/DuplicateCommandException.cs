using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices
{
    [Serializable]
    internal class DuplicateCommandException : ApplicationException
    {
        public DuplicateCommandException() { }
        public DuplicateCommandException(string message) : base(message) { }
        public DuplicateCommandException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public DuplicateCommandException(string message, Exception innerException) : base(message, innerException) { }
    }
}
