using System.Threading.Tasks;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    /// <summary>
    /// Defines an awaitable custom command module for an <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IAwaitableCommandModule
    {
        /// <summary>
        /// Executes a user-defined code block using the specified <see cref="ICommandExecutionContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ICommandExecutionContext"/> containing contextual information about the execution environment.</param>
        /// <returns></returns>
        Task<ICommandResponse> Execute(ICommandExecutionContext context);
    }
}
