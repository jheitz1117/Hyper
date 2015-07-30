using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandResponseSerializerFactory
    {
        ICommandResponseSerializer Create();
    }
}
