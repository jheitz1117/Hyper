using System;
using System.ServiceModel;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    /// <summary>
    /// <see cref="IServiceHostFactory"/> implementation to wrap a factory method that creates instances of <see cref="ServiceHost"/>.
    /// </summary>
    internal sealed class ServiceHostFactoryMethodWrapper : IServiceHostFactory
    {
        private readonly Func<ServiceHost> _factory;

        /// <summary>
        /// Initializes an instance of <see cref="ServiceHostFactoryMethodWrapper"/> using the specified factory method.
        /// </summary>
        /// <param name="factory">The delegate that is invoked to create the <see cref="ServiceHost"/> object.</param>
        public ServiceHostFactoryMethodWrapper(Func<ServiceHost> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates an instance of <see cref="ServiceHost"/> using the specified factory method.
        /// </summary>
        /// <returns></returns>
        public ServiceHost Create()
        {
            return _factory();
        }
    }
}
