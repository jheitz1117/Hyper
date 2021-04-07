using System.Configuration;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration.ConfigurationSections
{
    internal sealed class HyperNodeConfigurationSection : ConfigurationSection, IHyperNodeConfiguration
    {
        /// <summary>
        /// Specifies the name of the <see cref="HyperNodeService"/>. This attribute is required.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string HyperNodeName
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        /// <summary>
        /// Specifies whether the task progress cache will be enabled when the <see cref="HyperNodeService"/> starts.
        /// </summary>
        [ConfigurationProperty("enableTaskProgressCache", DefaultValue = HyperNodeService.DefaultTaskProgressCacheEnabled, IsRequired = false)]
        public bool? EnableTaskProgressCache
        {
            get => (bool)this["enableTaskProgressCache"];
            set => this["enableTaskProgressCache"] = value;
        }

        /// <summary>
        /// Specifies whether diagnostics will be enabled when the <see cref="HyperNodeService"/> starts.
        /// </summary>
        [ConfigurationProperty("enableDiagnostics", DefaultValue = HyperNodeService.DefaultDiagnosticsEnabled, IsRequired = false)]
        public bool? EnableDiagnostics
        {
            get => (bool)this["enableDiagnostics"];
            set => this["enableDiagnostics"] = value;
        }

        /// <summary>
        /// Specifies a starting value for the task progress cache duration when the <see cref="HyperNodeService"/> starts.
        /// </summary>
        [ConfigurationProperty("taskProgressCacheDuration", DefaultValue = HyperNodeService.DefaultProgressCacheDurationMinutes, IsRequired = false)]
        public int? TaskProgressCacheDurationMinutes
        {
            get => (int)this["taskProgressCacheDuration"];
            set => this["taskProgressCacheDuration"] = value;
        }

        /// <summary>
        /// Specifies an upper bound on the number of tasks that can run concurrently. A value of -1 means there is no upper bound.
        /// </summary>
        [ConfigurationProperty("maxConcurrentTasks", DefaultValue = HyperNodeService.DefaultMaxConcurrentTasks, IsRequired = false)]
        public int? MaxConcurrentTasks
        {
            get => (int)this["maxConcurrentTasks"];
            set => this["maxConcurrentTasks"] = value;
        }

        [ConfigurationProperty("taskIdProviderType", IsRequired = false)]
        public string TaskIdProviderType
        {
            get => (string)this["taskIdProviderType"];
            set => this["taskIdProviderType"] = value;
        }

        [ConfigurationProperty("hyperNodeEventHandlerType", IsRequired = false)]
        public string HyperNodeEventHandlerType
        {
            get => (string)this["hyperNodeEventHandlerType"];
            set => this["hyperNodeEventHandlerType"] = value;
        }

        /// <summary>
        /// This property (and all such collection properties) must be defined with a return type that matches the <see cref="ConfigurationCollectionAttribute"/> to satisfy the .NET framework.
        /// As such, we have to explicitly (and separately) implement the collection properties for the <see cref="IHyperNodeConfiguration"/> interface.
        /// </summary>
        [ConfigurationProperty("activityMonitors")]
        [ConfigurationCollection(typeof(ActivityMonitorElementCollection))]
        public ActivityMonitorElementCollection ActivityMonitors => this["activityMonitors"] as ActivityMonitorElementCollection;

        /// <summary>
        /// This property (and all such collection properties) must be defined with a return type that matches the <see cref="ConfigurationCollectionAttribute"/> to satisfy the .NET framework.
        /// As such, we have to explicitly (and separately) implement the collection properties for the <see cref="IHyperNodeConfiguration"/> interface.
        /// </summary>
        [ConfigurationProperty("systemCommands")]
        [ConfigurationCollection(typeof(SystemCommandElementCollection), AddItemName = "systemCommand")]
        public SystemCommandElementCollection SystemCommands => this["systemCommands"] as SystemCommandElementCollection;

        /// <summary>
        /// This property (and all such collection properties) must be defined with a return type that matches the <see cref="ConfigurationCollectionAttribute"/> to satisfy the .NET framework.
        /// As such, we have to explicitly (and separately) implement the collection properties for the <see cref="IHyperNodeConfiguration"/> interface.
        /// </summary>
        [ConfigurationProperty("commandModules")]
        [ConfigurationCollection(typeof(CommandModuleElementCollection))]
        public CommandModuleElementCollection CommandModules => this["commandModules"] as CommandModuleElementCollection;

        /// <summary>
        /// This property must be implemented explicitly because its sibling property of the same name must have a specific return type to satisfy the .NET framework.
        /// </summary>
        IActivityMonitorConfigurationCollection IHyperNodeConfiguration.ActivityMonitors => ActivityMonitors;

        /// <summary>
        /// This property must be implemented explicitly because its sibling property of the same name must have a specific return type to satisfy the .NET framework.
        /// </summary>
        ISystemCommandConfigurationCollection IHyperNodeConfiguration.SystemCommands => SystemCommands;

        /// <summary>
        /// This property must be implemented explicitly because its sibling property of the same name must have a specific return type to satisfy the .NET framework.
        /// </summary>
        ICommandModuleConfigurationCollection IHyperNodeConfiguration.CommandModules => CommandModules;
    }
}
