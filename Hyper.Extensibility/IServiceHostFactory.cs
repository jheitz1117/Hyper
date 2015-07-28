using System.ServiceModel;

namespace Hyper.Extensibility.WcfHosting
{
    public interface IServiceHostFactory
    {
        ServiceHost Create();
    }
}
