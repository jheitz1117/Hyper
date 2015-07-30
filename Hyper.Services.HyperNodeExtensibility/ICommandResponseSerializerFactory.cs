using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandResponseSerializerFactory
    {
        ICommandResponseSerializer Create();
    }
}
