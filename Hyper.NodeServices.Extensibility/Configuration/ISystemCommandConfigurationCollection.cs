using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface ISystemCommandConfigurationCollection : IEnumerable<ISystemCommandConfiguration>
    {
        bool Enabled { get; }
        ISystemCommandConfiguration GetByCommandName(string commandName);
    }
}