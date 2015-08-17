using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    /// <summary>
    /// Defines a method to create instances of <see cref="ICommandResponseSerializer"/>.
    /// </summary>
    public interface ICommandResponseSerializerFactory
    {
        /// <summary>
        /// Returns an instance of <see cref="ICommandResponseSerializer"/>.
        /// </summary>
        /// <returns></returns>
        ICommandResponseSerializer Create();
    }
}
