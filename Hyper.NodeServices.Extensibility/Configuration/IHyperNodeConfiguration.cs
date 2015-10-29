using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface IHyperNodeConfiguration
    {
        /// <summary>
        /// Specifies the name of the <see cref="IHyperNodeService"/>. This property is required.
        /// </summary>
        string HyperNodeName { get; set; }

        /// <summary>
        /// Specifies whether the task progress cache will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        bool? EnableTaskProgressCache { get; set; }

        /// <summary>
        /// Specifies whether diagnostics will be enabled when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        bool? EnableDiagnostics { get; set; }

        /// <summary>
        /// Specifies a starting value for the task progress cache duration when the <see cref="IHyperNodeService"/> starts.
        /// </summary>
        int? TaskProgressCacheDurationMinutes { get; set; }

        /// <summary>
        /// Specifies an upper bound on the number of tasks that can run concurrently. A value of -1 means there is no upper bound.
        /// </summary>
        int? MaxConcurrentTasks { get; set; }

        string TaskIdProviderType { get; set; }
        IActivityMonitorConfigurationCollection ActivityMonitors { get; }
        ISystemCommandConfigurationCollection SystemCommands { get; }
        ICommandModuleConfigurationCollection CommandModules { get; }
    }
}
