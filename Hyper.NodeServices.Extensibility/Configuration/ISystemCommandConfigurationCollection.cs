using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a collection of system-level <see cref="ICommandModule"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface ISystemCommandConfigurationCollection : IEnumerable<ISystemCommandConfiguration>
    {
        /// <summary>
        /// Indicates whether system-level <see cref="ICommandModule"/> objects in the <see cref="IHyperNodeService"/> will be enabled
        /// when the <see cref="IHyperNodeService"/> starts. This property can be overridden for a specific command by adding an
        /// instance of <see cref="ISystemCommandConfiguration"/> to the <see cref="ISystemCommandConfigurationCollection"/> with
        /// the desired configuration.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Gets the <see cref="ISystemCommandConfiguration"/> object with the specified name.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="ISystemCommandConfiguration"/> object to get.</param>
        /// <returns></returns>
        ISystemCommandConfiguration GetByCommandName(string commandName);

        /// <summary>
        /// Determines whether the <see cref="ISystemCommandConfigurationCollection"/> contains an instance of <see cref="ISystemCommandConfiguration"/> with the specified name.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="ISystemCommandConfiguration"/> object to locate in the <see cref="ISystemCommandConfigurationCollection"/>.</param>
        /// <returns></returns>
        bool ContainsCommandName(string commandName);
    }
}