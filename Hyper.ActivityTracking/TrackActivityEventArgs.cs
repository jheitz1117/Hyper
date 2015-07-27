using System;
using Hyper.Extensibility.ActivityTracking;

namespace Hyper.ActivityTracking
{
    public class TrackActivityEventArgs : EventArgs
    {
        private readonly IActivityItem  _activityItem;
        public IActivityItem ActivityItem
        {
            get { return _activityItem; }
        }

        public TrackActivityEventArgs(IActivityItem activityItem)
        {
            _activityItem = activityItem;
        }
    }
}
