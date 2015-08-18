using System;
using Hyper.Extensibility.ActivityTracking;

namespace Hyper.ActivityTracking
{
    /// <summary>
    /// Event arguments for <see cref="HyperActivityTracker"/> objects.
    /// </summary>
    public class TrackActivityEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="IActivityItem"/> describing the event.
        /// </summary>
        public IActivityItem ActivityItem
        {
            get { return _activityItem; }
        } private readonly IActivityItem  _activityItem;

        /// <summary>
        /// Initializes an instance of <see cref="TrackActivityEventArgs"/> using the specified <see cref="IActivityItem"/>.
        /// </summary>
        /// <param name="activityItem">The <see cref="IActivityItem"/> describing the event.</param>
        public TrackActivityEventArgs(IActivityItem activityItem)
        {
            _activityItem = activityItem;
        }
    }
}
