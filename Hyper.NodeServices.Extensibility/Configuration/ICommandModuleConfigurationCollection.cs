using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of a collection of user-defined <see cref="ICommandModule"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface ICommandModuleConfigurationCollection : IEnumerable<ICommandModuleConfiguration>
    {
        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandRequestSerializer"/>. If this property is supplied,
        /// the specified type serves as the default <see cref="ICommandRequestSerializer"/> implementation for all user-defined
        /// <see cref="ICommandModule"/> objects in the <see cref="ICommandModuleConfigurationCollection"/>. This collection-level
        /// default can be overridden for a specific command by supplying the desired <see cref="ICommandRequestSerializer"/> type
        /// string in the <see cref="ICommandModuleConfiguration.RequestSerializerType"/> property for the <see cref="ICommandModuleConfiguration"/>
        /// for that command.
        /// </summary>
        string RequestSerializerType { get; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandResponseSerializer"/>. If this property is supplied,
        /// the specified type serves as the default <see cref="ICommandResponseSerializer"/> implementation for all user-defined
        /// <see cref="ICommandModule"/> objects in the <see cref="ICommandModuleConfigurationCollection"/>. This collection-level
        /// default can be overridden for a specific command by supplying the desired <see cref="ICommandResponseSerializer"/> type
        /// string in the <see cref="ICommandModuleConfiguration.ResponseSerializerType"/> property for the <see cref="ICommandModuleConfiguration"/>
        /// for that command.
        /// </summary>
        string ResponseSerializerType { get; }
    }
}