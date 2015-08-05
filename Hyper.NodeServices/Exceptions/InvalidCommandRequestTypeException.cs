using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices
{
    [Serializable]
    internal class InvalidCommandRequestTypeException : ApplicationException
    {
        public InvalidCommandRequestTypeException() { }
        public InvalidCommandRequestTypeException(Type expectedType, Type actualType)
            : base(string.Format("Request type '{0}' could not be converted to type '{1}'.", actualType.FullName, expectedType.FullName)) { }
        public InvalidCommandRequestTypeException(string message) : base(message) { }
        public InvalidCommandRequestTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidCommandRequestTypeException(string message, Exception innerException) : base(message, innerException) { }

    }
}
