using System;

namespace Hyper.Extensibility.ActivityTracking
{
    public interface IActivityItem
    {
        DateTime EventDateTime { get; set; }
        string Agent { get; set; }
        string EventDescription { get; set; }
        string EventDetail { get; set; }
    }
}
