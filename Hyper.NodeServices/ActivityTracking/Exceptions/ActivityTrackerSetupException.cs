using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.ActivityTracking
{
    [Serializable]
    internal class ActivityTrackerSetupException : ApplicationException
    {
        public ActivityTrackerSetupException() { }
        public ActivityTrackerSetupException(string message) : base(message) { }
        public ActivityTrackerSetupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ActivityTrackerSetupException(string message, Exception innerException) : base(message, innerException) { }
    }
}
