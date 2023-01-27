using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Communicates with a remote implementation of the <see cref="IHyperNodeService"/> service contract.
    /// </summary>
    public class HyperNodeClient : IHyperNodeService, IDisposable
    {
        private static readonly ConcurrentDictionary<string, ChannelFactory<IHyperNodeService>> ChannelFactories = new ConcurrentDictionary<string, ChannelFactory<IHyperNodeService>>();
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
            // Only construct a factory if necessary. Otherwise, grab the cached one.
            var factory = ChannelFactories.GetOrAdd(nodeName, key => new ChannelFactory<IHyperNodeService>(key));

            // Each instance gets its own channel.
            _channel = factory.CreateChannel();
        }

        /// <summary>
        /// Processes and/or forwards the specified message.
        /// </summary>
        /// <param name="message">The <see cref="HyperNodeMessageRequest"/> object to process.</param>
        /// <returns></returns>
        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            return _channel.ProcessMessageAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Processes and/or forwards the specified message.
        /// </summary>
        /// <param name="message">The <see cref="HyperNodeMessageRequest"/> object to process.</param>
        /// <returns></returns>
        public async Task<HyperNodeMessageResponse> ProcessMessageAsync(HyperNodeMessageRequest message)
        {
            return await _channel.ProcessMessageAsync(message);
        }

        /// <summary>
        /// Disposes the internal <see cref="IClientChannel"/> associated with this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// When overridden in a derived class, disposes of resources consumed by this instance.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed. Otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var client = _channel as IClientChannel;

                try
                {
                    client?.Close();
                }
                catch
                {
                    client?.Abort();
                }
                finally
                {
                    client?.Dispose();
                }
            }
        }
    }
}
