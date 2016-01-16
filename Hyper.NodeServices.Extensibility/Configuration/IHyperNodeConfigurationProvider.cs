namespace Hyper.NodeServices.Extensibility.Configuration
{
    /// <summary>
    /// Defines a method to provide an instance of <see cref="IHyperNodeConfiguration"/>.
    /// </summary>
    public interface IHyperNodeConfigurationProvider
    {
        /// <summary>
        /// Constructs an instance of <see cref="IHyperNodeConfiguration"/> and populates it with data.
        /// </summary>
        /// <returns></returns>
        IHyperNodeConfiguration GetConfiguration();
    }
}