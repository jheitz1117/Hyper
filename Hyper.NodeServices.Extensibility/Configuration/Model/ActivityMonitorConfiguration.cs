using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of a user-defined <see cref="HyperNodeServiceActivityMonitor"/> object in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class ActivityMonitorConfiguration : IActivityMonitorConfiguration
    {
        /// <summary>
        /// The name of the <see cref="HyperNodeServiceActivityMonitor"/> object. This property is required.
        /// </summary>
        public string MonitorName { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that inherits <see cref="HyperNodeServiceActivityMonitor"/>.
        /// This property is required.
        /// </summary>
        public string MonitorType { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="HyperNodeServiceActivityMonitor"/> object will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
