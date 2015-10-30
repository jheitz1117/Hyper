namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class ActivityMonitorConfiguration : IActivityMonitorConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
    }
}
