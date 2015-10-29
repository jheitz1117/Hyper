using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface IActivityMonitorConfigurationCollection : IEnumerable<IActivityMonitorConfiguration>
    {
        // Right now, this interface doesn't expose any additional configuration properties apart from the enumerator provided by IEnumerable.
        // However, I'm leaving this open for future extensibility just in case.
    }
}