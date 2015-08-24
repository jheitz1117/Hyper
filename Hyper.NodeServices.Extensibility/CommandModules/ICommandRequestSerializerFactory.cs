using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    /// <summary>
    /// Defines a method to create instances of <see cref="ICommandRequestSerializer"/>.
    /// </summary>
    public interface ICommandRequestSerializerFactory
    {
        /// <summary>
        /// Returns an instance of <see cref="ICommandRequestSerializer"/>.
        /// </summary>
        /// <returns></returns>
        ICommandRequestSerializer Create();
    }
}
