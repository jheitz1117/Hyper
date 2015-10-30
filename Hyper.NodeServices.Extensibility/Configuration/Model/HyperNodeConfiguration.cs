namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class HyperNodeConfiguration : IHyperNodeConfiguration
    {
        public string HyperNodeName { get; set; }
        public bool? EnableTaskProgressCache { get; set; }
        public bool? EnableDiagnostics { get; set; }
        public int? TaskProgressCacheDurationMinutes { get; set; }
        public int? MaxConcurrentTasks { get; set; }
        public string TaskIdProviderType { get; set; }
        public IActivityMonitorConfigurationCollection ActivityMonitors { get; set; }
        public ISystemCommandConfigurationCollection SystemCommands { get; set; }
        public ICommandModuleConfigurationCollection CommandModules { get; set; }

        public HyperNodeConfiguration()
        {
            this.ActivityMonitors = new ActivityMonitorConfigurationCollection();
            this.SystemCommands = new SystemCommandConfigurationCollection();
            this.CommandModules = new CommandModuleConfigurationCollection();
        }
    }
}
