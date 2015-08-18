using System;

namespace Hyper.Extensibility.ActivityTracking
{
    /// <summary>
    /// Defines basic properties for arbitrary activity events.
    /// </summary>
    public interface IActivityItem
    {
        /// <summary>
        /// The date and time the event happened.
        /// </summary>
        DateTime EventDateTime { get; set; }

        /// <summary>
        /// The name of the agent reporting the activity event.
        /// </summary>
        string Agent { get; set; }

        /// <summary>
        /// A description of the activity event.
        /// </summary>
        string EventDescription { get; set; }

        /// <summary>
        /// A longer, more detailed description of the activity event.
        /// </summary>
        string EventDetail { get; set; }
    }
}
