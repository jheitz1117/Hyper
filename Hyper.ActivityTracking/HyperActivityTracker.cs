namespace Hyper.ActivityTracking
{
    public delegate void TrackActivityEventHandler(object sender, TrackActivityEventArgs e);

    public abstract class HyperActivityTracker
    {
        private readonly object _lock = new object();

        public event TrackActivityEventHandler TrackActivityHandler = (sender, args) => { };

        protected virtual void OnTrackActivity(TrackActivityEventArgs e)
        {
            TrackActivityEventHandler handler;
            lock (_lock)
            {
                handler = TrackActivityHandler;
            }
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
