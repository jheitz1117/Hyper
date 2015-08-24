using System;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.ActivityTracking
{
    /// <summary>
    /// Defines an abstract implementation of <see cref="IObserver{T}"/> that tracks <see cref="IHyperNodeActivityEventItem"/> objects.
    /// </summary>
    public abstract class HyperNodeServiceActivityMonitor : IObserver<IHyperNodeActivityEventItem>
    {
        /// <summary>
        /// The name of this <see cref="HyperNodeServiceActivityMonitor"/>. If no name is specified, or if it is set
        /// to null or whitespace, then a new <see cref="Guid"/> is used.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = (string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString() : value);
            }
        } private string _name = Guid.NewGuid().ToString();

        /// <summary>
        /// Specifies whether this <see cref="HyperNodeServiceActivityMonitor"/> is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        } private bool _enabled = true;

        /// <summary>
        /// When overridden in a derived class, provides the opportunity to run custom initialization code for <see cref="HyperNodeServiceActivityMonitor"/>
        /// implementations. This method is called immediately after the <see cref="HyperNodeServiceActivityMonitor"/> instance is instantiated. Once the
        /// <see cref="HyperNodeServiceActivityMonitor"/> has been initialized, it exists for the lifetime of the <see cref="IHyperNodeService"/>. 
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// When overridden in a derived class, provides a way to filter which <see cref="IHyperNodeActivityEventItem"/> objects should be tracked.
        /// By default, all activity events are tracked.
        /// </summary>
        /// <param name="activity">The <see cref="IHyperNodeActivityEventItem"/> to examine.</param>
        /// <returns></returns>
        public virtual bool ShouldTrack(IHyperNodeActivityEventItem activity) { return true; }

        /// <summary>
        /// When overridden in a derived class, tracks <see cref="IHyperNodeActivityEventItem"/> objects. This method must not call the
        /// <see cref="OnNext(IHyperNodeActivityEventItem)"/> method.
        /// </summary>
        /// <param name="activity">The <see cref="IHyperNodeActivityEventItem"/> to track.</param>
        public abstract void OnTrack(IHyperNodeActivityEventItem activity);

        #region IObserver implementations

        /// <summary>
        /// Consumes <see cref="IHyperNodeActivityEventItem"/> objects by delegating them to the <see cref="OnTrack(IHyperNodeActivityEventItem)"/> method.
        /// This method should not be called from the <see cref="OnTrack(IHyperNodeActivityEventItem)"/> method.
        /// </summary>
        /// <param name="activity">The <see cref="IHyperNodeActivityEventItem"/> to consume.</param>
        public void OnNext(IHyperNodeActivityEventItem activity) { OnTrack(activity); }

        /// <summary>
        /// When overridden in a derived class, handles exceptions thrown by a <see cref="IObservable{T}"/> to which this <see cref="HyperNodeServiceActivityMonitor"/> is subscribed.
        /// By default, this method rethrows the <see cref="Exception"/>.
        /// </summary>
        /// <param name="error">The <see cref="Exception"/> to handle.</param>
        public virtual void OnError(Exception error) { throw error; }

        /// <summary>
        /// When overridden in a derived class, provides an opportunity to clean up any resources as a result of the event stream completing.
        /// </summary>
        public virtual void OnCompleted() { /* Event streams never complete, so do nothing */ }

        #endregion
    }
}
