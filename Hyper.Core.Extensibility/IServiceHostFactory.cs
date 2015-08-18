using System.ServiceModel;

namespace Hyper.Extensibility.WcfHosting
{
    /// <summary>
    /// Defines a method to create <see cref="ServiceHost"/> instances.
    /// </summary>
    public interface IServiceHostFactory
    {
        /// <summary>
        /// Creates a <see cref="ServiceHost"/> instance.
        /// </summary>
        /// <returns></returns>
        ServiceHost Create();
    }
}
