using System;

namespace Hyper.NodeServices.Extensibility
{
    public abstract class HyperNodeServiceActivityMonitor : IObserver<IHyperNodeActivityEventItem>
    {
        /// <summary>
        /// Allows client code to obtain a reference this HyperNodeServiceMonitor instance
        /// </summary>
        private string _name = Guid.NewGuid().ToString();
        public string Name
        {
            get { return _name; }
            set
            {
                _name = (string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString() : value);
            }
        }

        /// <summary>
        /// Allows this activity monitor to be turned on and off
        /// </summary>
        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        // Provides a way for an activity monitor to perform any internal setup necessary before use
        public virtual void Initialize() { }

        // Provides a way for the monitor to filter out activity items it doesn't care about
        public virtual bool ShouldTrack(IHyperNodeActivityEventItem activity) { return true; }

        /* IObserver<T> implementations */
        public abstract void OnNext(IHyperNodeActivityEventItem activity);
        public virtual void OnError(Exception error) { throw error; }
        public virtual void OnCompleted() { }
    }
}
