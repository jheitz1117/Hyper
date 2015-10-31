using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class HyperNodeConfiguration : IHyperNodeConfiguration
    {
        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/>. This property is required.
        /// </summary>
        public string HyperNodeName { get; set; }

        /// <summary>
        /// Indicates whether the task progress cache will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// If this property is null, the service will choose whether to enable the task progress cache.
        /// </summary>
        public bool? EnableTaskProgressCache { get; set; }

        /// <summary>
        /// Indicates whether diagnostics will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// If this property is null, the service will choose whether to enable diagnostics.
        /// </summary>
        public bool? EnableDiagnostics { get; set; }

        /// <summary>
        /// The duration, in minutes, of items in the task progress cache.
        /// If this property is null, a default value is used.
        /// </summary>
        public int? TaskProgressCacheDurationMinutes { get; set; }

        /// <summary>
        /// The maximum number of tasks that can run concurrently in the <see cref="IHyperNodeService"/>. A value of -1 means unlimited.
        /// If this property is null, a default value is used.
        /// </summary>
        public int? MaxConcurrentTasks { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ITaskIdProvider"/>. If this property is not supplied, a default
        /// implementation is used by the <see cref="IHyperNodeService"/>. Otherwise, an instance of the specified type will be used instead.
        /// </summary>
        public string TaskIdProviderType { get; set; }

        /// <summary>
        /// A collection of <see cref="IActivityMonitorConfiguration"/> objects specifying how user-defined <see cref="HyperNodeServiceActivityMonitor"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        IActivityMonitorConfigurationCollection IHyperNodeConfiguration.ActivityMonitors
        {
            get { return this.ActivityMonitors; }
        }

        /// <summary>
        /// A collection of <see cref="ISystemCommandConfiguration"/> objects specifying how system-level <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        ISystemCommandConfigurationCollection IHyperNodeConfiguration.SystemCommands
        {
            get { return this.SystemCommands; }
        }

        /// <summary>
        /// A collection of <see cref="ICommandModuleConfiguration"/> objects specifying how user-defined <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        ICommandModuleConfigurationCollection IHyperNodeConfiguration.CommandModules
        {
            get { return this.CommandModules; }
        }

        /// <summary>
        /// A collection of <see cref="ActivityMonitorConfiguration"/> objects specifying how user-defined <see cref="HyperNodeServiceActivityMonitor"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        public ActivityMonitorConfigurationCollection ActivityMonitors { get; set; }

        /// <summary>
        /// A collection of <see cref="SystemCommandConfiguration"/> objects specifying how system-level <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        public SystemCommandConfigurationCollection SystemCommands { get; set; }

        /// <summary>
        /// A collection of <see cref="CommandModuleConfiguration"/> objects specifying how user-defined <see cref="ICommandModule"/> objects should be configured in the <see cref="IHyperNodeService"/>.
        /// </summary>
        public CommandModuleConfigurationCollection CommandModules { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfiguration"/> class.
        /// </summary>
        public HyperNodeConfiguration()
        {
            this.ActivityMonitors = new ActivityMonitorConfigurationCollection();
            this.SystemCommands = new SystemCommandConfigurationCollection();
            this.CommandModules = new CommandModuleConfigurationCollection();
        }
    }
}
