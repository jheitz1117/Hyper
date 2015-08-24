using System.Configuration;

namespace Hyper.NodeServices.Configuration
{
    internal sealed class HyperNodeConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Specifies the name of the <see cref="HyperNodeService"/>. This attribute is required.
        /// The default value is "HyperNode1".
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "HyperNode1", IsRequired = true)]
        public string HyperNodeName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Specifies whether the task progress cache will be enabled when the <see cref="HyperNodeService"/> starts.
        /// The cache is not enabled by default.
        /// </summary>
        [ConfigurationProperty("enableTaskProgressCache", DefaultValue = false, IsRequired = false)]
        public bool EnableTaskProgressCache
        {
            get { return (bool)this["enableTaskProgressCache"]; }
            set { this["enableTaskProgressCache"] = value; }
        }

        /// <summary>
        /// Specifies whether diagnostics will be enabled when the <see cref="HyperNodeService"/> starts.
        /// Diagnostics are not enabled by default.
        /// </summary>
        [ConfigurationProperty("enableDiagnostics", DefaultValue = false, IsRequired = false)]
        public bool EnableDiagnostics
        {
            get { return (bool)this["enableDiagnostics"]; }
            set { this["enableDiagnostics"] = value; }
        }

        /// <summary>
        /// Specifies a starting value for the task progress cache duration when the <see cref="HyperNodeService"/> starts.
        /// The default value is 60 minutes.
        /// </summary>
        [ConfigurationProperty("taskProgressCacheDuration", DefaultValue = 60, IsRequired = false)]
        public int TaskProgressCacheDurationMinutes
        {
            get { return (int)this["taskProgressCacheDuration"]; }
            set { this["taskProgressCacheDuration"] = value; }
        }

        /// <summary>
        /// Specifies an upper bound on the number of tasks that can run concurrently. A value of -1 means there is no upper bound.
        /// No upper bound is specified by default.
        /// </summary>
        [ConfigurationProperty("maxConcurrentTasks", DefaultValue = -1, IsRequired = false)]
        public int MaxConcurrentTasks
        {
            get { return (int)this["maxConcurrentTasks"]; }
            set { this["maxConcurrentTasks"] = value; }
        }

        [ConfigurationProperty("taskIdProviderType", IsRequired = false)]
        public string TaskIdProviderType
        {
            get { return (string)this["taskIdProviderType"]; }
            set { this["taskIdProviderType"] = value; }
        }

        [ConfigurationProperty("activityMonitors")]
        [ConfigurationCollection(typeof(ActivityMonitorElementCollection))]
        public ActivityMonitorElementCollection ActivityMonitors
        {
            get { return this["activityMonitors"] as ActivityMonitorElementCollection; }
        }

        [ConfigurationProperty("systemCommands")]
        [ConfigurationCollection(typeof(SystemCommandElementCollection), AddItemName = "systemCommand")]
        public SystemCommandElementCollection SystemCommands
        {
            get { return this["systemCommands"] as SystemCommandElementCollection; }
        }

        [ConfigurationProperty("commandModules")]
        [ConfigurationCollection(typeof(CommandModuleElementCollection))]
        public CommandModuleElementCollection CommandModules
        {
            get { return this["commandModules"] as CommandModuleElementCollection; }
        }
    }
}
