namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface ICommandModuleConfiguration
    {
        string Name { get; set; }
        string Type { get; set; }
        bool Enabled { get; set; }
        string RequestSerializerType { get; set; }
        string ResponseSerializerType { get; set; }
    }
}
