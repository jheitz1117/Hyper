using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility
{
    /// <summary>
    /// Creates IDs for tasks initiated by <see cref="IHyperNodeService"/> instances. The Task ID must be unique relative to all other tasks that are currently executing.
    /// Once a task has completed, its ID may be reused.
    /// </summary>
    public interface ITaskIdProvider
    {
        /// <summary>
        /// Provides the opportunity to run custom initialization code for <see cref="ITaskIdProvider"/> implementations. This method is called
        /// immediately after the <see cref="ITaskIdProvider"/> instance is instantiated. Once the <see cref="ITaskIdProvider"/> has been initialized,
        /// it exists for the lifetime of the <see cref="IHyperNodeService"/>. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Creates a Task ID using the specified <see cref="IReadOnlyHyperNodeMessageInfo"/> object.
        /// </summary>
        /// <param name="message">The <see cref="IReadOnlyHyperNodeMessageInfo"/> to use.</param>
        /// <returns></returns>
        string CreateTaskId(IReadOnlyHyperNodeMessageInfo message);
    }
}
