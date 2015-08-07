using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public interface ICommandResponseSerializerFactory
    {
        ICommandResponseSerializer Create();
    }
}
