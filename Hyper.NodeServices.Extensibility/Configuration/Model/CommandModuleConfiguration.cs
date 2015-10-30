namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class CommandModuleConfiguration : ICommandModuleConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public string RequestSerializerType { get; set; }
        public string ResponseSerializerType { get; set; }
    }
}
