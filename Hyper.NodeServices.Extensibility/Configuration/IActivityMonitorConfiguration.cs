namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface IActivityMonitorConfiguration
    {
        string Name { get; set; }
        string Type { get; set; }
        bool Enabled { get; set; }
    }
}
