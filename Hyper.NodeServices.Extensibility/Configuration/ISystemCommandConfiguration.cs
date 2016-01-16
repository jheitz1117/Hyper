using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a system-level <see cref="ICommandModule"/> object in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface ISystemCommandConfiguration
    {
        /// <summary>
        /// The name of the system-level <see cref="ICommandModule"/> object. This property is required.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Indicates whether the system-level <see cref="ICommandModule"/> object will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        bool Enabled { get; }
    }
}
