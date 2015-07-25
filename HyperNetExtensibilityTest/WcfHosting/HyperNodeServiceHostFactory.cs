using System.ServiceModel;
using Hyper.Services.HyperNodeServices;
using Hyper.WcfHosting;

namespace HyperNetExtensibilityTest.WcfHosting
{
    public class HyperNodeServiceHostFactory : IServiceHostFactory
    {
        public ServiceHost Create()
        {
            var service = HyperNodeService.Create();
            var host = new CancellableServiceHost(service);
            host.RegisterCancellationDelegate(service.Cancel);

            return host;
        }
    }
}