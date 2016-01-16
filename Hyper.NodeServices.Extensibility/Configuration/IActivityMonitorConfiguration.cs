using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a user-defined <see cref="HyperNodeServiceActivityMonitor"/> object in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IActivityMonitorConfiguration
    {
        /// <summary>
        /// The name of the <see cref="HyperNodeServiceActivityMonitor"/> object. This property is required.
        /// </summary>
        string MonitorName { get; }

        /// <summary>
        /// The assembly qualified name of a type that inherits <see cref="HyperNodeServiceActivityMonitor"/>.
        /// This property is required.
        /// </summary>
        string MonitorType { get; }

        /// <summary>
        /// Indicates whether the <see cref="HyperNodeServiceActivityMonitor"/> object will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        bool Enabled { get; }
    }
}
