using System;

namespace Hyper.NodeServices.Extensibility
{
    public interface ITaskActivityTracker
    {
        void Track(string eventDescription);
        void Track(string eventDescription, string eventDetail);
        void Track(string eventDescription, string eventDetail, object eventData);
        void Track(string eventDescription, double? progressPart, double? progressTotal);
        void Track(string eventDescription, string eventDetail, double? progressPart, double? progressTotal);
        void Track(string eventDescription, string eventDetail, object eventData, double? progressPart, double? progressTotal);
        void TrackFormat(string eventDescription, params object[] args);
        void TrackException(Exception exception);
    }
}
