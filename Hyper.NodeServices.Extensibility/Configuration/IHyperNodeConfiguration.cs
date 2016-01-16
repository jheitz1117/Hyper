using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines configurable properties of an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IHyperNodeConfiguration
    {
        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/>. This property is required.
        /// </summary>
        string HyperNodeName { get; }

        /// <summary>
        /// Indicates whether the task progress cache will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// If this property is null, the service will choose whether to enable the task progress cache.
        /// </summary>
        bool? EnableTaskProgressCache { get; }

        /// <summary>
        /// Indicates whether diagnostics will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// If this property is null, the service will choose whether to enable diagnostics.
        /// </summary>
        bool? EnableDiagnostics { get; }

        /// <summary>
        /// The duration, in minutes, of items in the task progress cache.
        /// If this property is null, a default value is used.
        /// </summary>
        int? TaskProgressCacheDurationMinutes { get; }

        /// <summary>
        /// The maximum number of tasks that can run concurrently in the <see cref="IHyperNodeService"/>. A value of -1 means unlimited.
        /// If this property is null, a default value is used.
        /// </summary>
        int? MaxConcurrentTasks { get; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ITaskIdProvider"/>. If this property is not supplied, a default
        /// implementation is used by the <see cref="IHyperNodeService"/>. Otherwise, an instance of the specified type will be used instead.
        /// </summary>
        string TaskIdProviderType { get; }

        /// <summary>
        /// A collection of <see cref="IActivityMonitorConfiguration"/> objects specifying how user-defined <see cref="HyperNodeServiceActivityMonitor"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        IActivityMonitorConfigurationCollection ActivityMonitors { get; }

        /// <summary>
        /// A collection of <see cref="ISystemCommandConfiguration"/> objects specifying how system-level <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        ISystemCommandConfigurationCollection SystemCommands { get; }

        /// <summary>
        /// A collection of <see cref="ICommandModuleConfiguration"/> objects specifying how user-defined <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        ICommandModuleConfigurationCollection CommandModules { get; }
    }
}
