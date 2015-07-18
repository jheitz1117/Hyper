using System.ServiceModel;

namespace Hyper.WcfHosting
{
    public interface IServiceHostFactory
    {
        ServiceHost Create();
    }
}
