using System.ServiceModel;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Client
{
    public class HyperNodeClient : IHyperNodeService
    {
        private readonly IHyperNodeService _channel;

        /// <summary>
        /// Creates an instance of <see cref="HyperNodeClient"/>. This overload assumes the configuration file contains a single endpoint for the <see cref="IHyperNodeService"/> contract.
        /// </summary>
        public HyperNodeClient()
            : this("*")
        { }

        /// <summary>
        /// Creates an instance of <see cref="HyperNodeClient"/>. This overload assumes that one or more endpoints have been defined in the configuration file for the <see cref="IHyperNodeService"/> contract.
        /// </summary>
        /// <param name="nodeName">The name of the <see cref="IHyperNodeService"/> endpoint to use.</param>
        public HyperNodeClient(string nodeName)
        {
            _channel = new ChannelFactory<IHyperNodeService>(nodeName).CreateChannel();
        }

        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            return _channel.ProcessMessage(message);
        }
    }
}
