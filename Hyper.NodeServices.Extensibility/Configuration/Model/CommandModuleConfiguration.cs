using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of a user-defined <see cref="ICommandModule"/> object in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class CommandModuleConfiguration : ICommandModuleConfiguration
    {
        /// <summary>
        /// The name of the user-defined <see cref="ICommandModule"/> object. This property is required.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandModule"/>.
        /// This property is required.
        /// </summary>
        public string CommandModuleType { get; set; }

        /// <summary>
        /// Indicates whether the user-defined <see cref="ICommandModule"/> object will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandRequestSerializer"/>. If this property is
        /// not supplied, the collection-level default is used instead. If no collection-level default was specified, then the
        /// <see cref="IHyperNodeService"/> chooses a default implemention to use.
        /// </summary>
        public string RequestSerializerType { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandResponseSerializer"/>. If this property is
        /// not supplied, the collection-level default is used instead. If no collection-level default was specified, then the
        /// <see cref="IHyperNodeService"/> chooses a default implemention to use.
        /// </summary>
        public string ResponseSerializerType { get; set; }
    }
}
