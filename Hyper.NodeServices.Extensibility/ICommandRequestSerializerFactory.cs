using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility
{
    public interface ICommandRequestSerializerFactory
    {
        ICommandRequestSerializer Create();
    }
}
