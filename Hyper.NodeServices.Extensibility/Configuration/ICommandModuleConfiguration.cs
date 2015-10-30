namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface ICommandModuleConfiguration
    {
        string Name { get; }
        string Type { get; }
        bool Enabled { get; }
        string RequestSerializerType { get; }
        string ResponseSerializerType { get; }
    }
}
