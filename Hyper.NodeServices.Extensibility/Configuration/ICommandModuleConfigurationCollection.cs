using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration
{
    // TODO: XDOC this interface
    public interface ICommandModuleConfigurationCollection : IEnumerable<ICommandModuleConfiguration>
    {
        string RequestSerializerType { get; set; }
        string ResponseSerializerType { get; set; }
    }
}