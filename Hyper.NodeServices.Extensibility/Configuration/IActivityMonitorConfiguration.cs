namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface IActivityMonitorConfiguration
    {
        string Name { get; }
        string Type { get; }
        bool Enabled { get; }
    }
}
