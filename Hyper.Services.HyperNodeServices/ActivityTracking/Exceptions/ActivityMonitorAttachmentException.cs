using System;
using System.Runtime.Serialization;

namespace Hyper.Services.HyperNodeServices.ActivityTracking
{
    [Serializable]
    internal class ActivityMonitorAttachmentException : ApplicationException
    {
        public ActivityMonitorAttachmentException() { }
        public ActivityMonitorAttachmentException(string message) : base(message) { }
        public ActivityMonitorAttachmentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ActivityMonitorAttachmentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
