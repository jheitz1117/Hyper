using System;
using System.Runtime.Serialization;

namespace Hyper.Services.HyperNodeServices.ActivityTracking
{
    [Serializable]
    public class ActivityTrackerSetupException : ApplicationException
    {
        public ActivityTrackerSetupException() { }
        public ActivityTrackerSetupException(string message) : base(message) { }
        public ActivityTrackerSetupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ActivityTrackerSetupException(string message, Exception innerException) : base(message, innerException) { }
    }
}
