using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices
{
    [Serializable]
    internal class ActivityMonitorSubscriptionException : Exception
    {
        public ActivityMonitorSubscriptionException() { }
        public ActivityMonitorSubscriptionException(string message) : base(message) { }
        public ActivityMonitorSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ActivityMonitorSubscriptionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
