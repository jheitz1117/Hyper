using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a collection of user-defined <see cref="HyperNodeServiceActivityMonitor"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IActivityMonitorConfigurationCollection : IEnumerable<IActivityMonitorConfiguration>
    {
        // Right now, this interface doesn't expose any additional configuration properties apart from the enumerator provided by IEnumerable.
        // However, I'm leaving this open for future extensibility just in case.
    }
}