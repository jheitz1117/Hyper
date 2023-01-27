using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a user-defined <see cref="ICommandModule"/> object in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface ICommandModuleConfiguration
    {
        /// <summary>
        /// The name of the user-defined <see cref="ICommandModule"/> object. This property is required.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandModule"/>.
        /// This property is required.
        /// </summary>
        string CommandModuleType { get; }

        /// <summary>
        /// Indicates whether the user-defined <see cref="ICommandModule"/> object will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandRequestSerializer"/>. If this property is
        /// not supplied, the collection-level default is used instead. If no collection-level default was specified, then the
        /// <see cref="IHyperNodeService"/> chooses a default implementation to use.
        /// </summary>
        string RequestSerializerType { get; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandResponseSerializer"/>. If this property is
        /// not supplied, the collection-level default is used instead. If no collection-level default was specified, then the
        /// <see cref="IHyperNodeService"/> chooses a default implementation to use.
        /// </summary>
        string ResponseSerializerType { get; }
    }
}
