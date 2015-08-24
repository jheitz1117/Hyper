using System.ServiceModel;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Communicates with a remote implementation of the <see cref="IHyperNodeService"/> service contract.
    /// </summary>
    public class HyperNodeClient : IHyperNodeService
    {
        private readonly IHyperNodeService _channel;

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeClient"/>. This overload assumes the configuration file contains a single endpoint for the <see cref="IHyperNodeService"/> contract.
        /// </summary>
        public HyperNodeClient()
            : this("*")
        { }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeClient"/>. This overload assumes that one or more endpoints have been defined in the configuration file for the <see cref="IHyperNodeService"/> contract. At least one of them must have a name matching <paramref name="nodeName"/>.
        /// </summary>
        /// <param name="nodeName">The name of the <see cref="IHyperNodeService"/> endpoint to use.</param>
        public HyperNodeClient(string nodeName)
        {
            _channel = new ChannelFactory<IHyperNodeService>(nodeName).CreateChannel();
        }

        /// <summary>
        /// Processes and/or forwards the specified message.
        /// </summary>
        /// <param name="message">The <see cref="HyperNodeMessageRequest"/> object to process.</param>
        /// <returns></returns>
        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            return _channel.ProcessMessage(message);
        }
    }
}
