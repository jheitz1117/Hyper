using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.Test
{
    public class HyperNodeMockConfigurationProvider : IHyperNodeConfigurationProvider
    {
        private readonly IHyperNodeConfiguration _config;

        public HyperNodeMockConfigurationProvider(IHyperNodeConfiguration config)
        {
            _config = config;
        }
        
        public IHyperNodeConfiguration GetConfiguration()
        {
            return _config;
        }
    }
}
