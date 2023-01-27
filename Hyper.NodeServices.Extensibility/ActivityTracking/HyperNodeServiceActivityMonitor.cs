using System;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.ActivityTracking
{
    /// <summary>
    /// Defines a class that processes <see cref="IHyperNodeActivityEventItem"/> objects.
    /// </summary>
    public abstract class HyperNodeServiceActivityMonitor
    {
        /// <summary>
        /// The name of this <see cref="HyperNodeServiceActivityMonitor"/>. If no name is specified, or if it is set
        /// to null or whitespace, then a new <see cref="Guid"/> is used.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString() : value;
        } private string _name = Guid.NewGuid().ToString();

        /// <summary>
        /// Specifies whether this <see cref="HyperNodeServiceActivityMonitor"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// When overridden in a derived class, runs custom initialization code for <see cref="HyperNodeServiceActivityMonitor"/>
        /// implementations. This method is called immediately after the <see cref="HyperNodeServiceActivityMonitor"/> instance is instantiated. Once the
        /// <see cref="HyperNodeServiceActivityMonitor"/> has been initialized, it exists for the lifetime of the <see cref="IHyperNodeService"/>. 
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// When overridden in a derived class, filters which <see cref="IHyperNodeActivityEventItem"/> objects should be tracked.
        /// By default, all activity events are tracked.
        /// </summary>
        /// <param name="activity">The <see cref="IHyperNodeActivityEventItem"/> to examine.</param>
        /// <returns></returns>
        public virtual bool ShouldTrack(IHyperNodeActivityEventItem activity) { return true; }

        /// <summary>
        /// When overridden in a derived class, processes <see cref="IHyperNodeActivityEventItem"/> objects.
        /// </summary>
        /// <param name="activity">The <see cref="IHyperNodeActivityEventItem"/> to track.</param>
        public abstract void OnTrack(IHyperNodeActivityEventItem activity);

        /// <summary>
        /// When overridden in a derived class, handles exceptions thrown by the internal activity reporting mechanism. Errors of this kind are such
        /// that they cannot be processed by the <see cref="OnTrack"/> method normally. These exceptions must be handled gracefully.
        /// </summary>
        /// <param name="exception"></param>
        public abstract void OnActivityReportingError(Exception exception);

        /// <summary>
        /// When overridden in a derived class, executes when the task completes.
        /// </summary>
        public virtual void OnTaskCompleted() { }
    }
}
