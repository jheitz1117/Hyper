using System;
using System.Threading;

namespace Hyper.Tasks
{
    public abstract class ManagedTaskBase : IManagedTask
    {
        /// <summary>
        /// Indicates whether or not the current task is already running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Handler for any exceptions thrown by the ExecuteTask method
        /// </summary>
        public IManagedTaskExceptionHandler ExceptionHandler { get; set; }

        /// <summary>
        /// Core functionality of this task
        /// </summary>
        protected abstract void Run(CancellationToken token);

        public void ExecuteTask(CancellationToken token)
        {
            if (this.IsRunning) { throw new InvalidOperationException("Task is already running."); }

            this.IsRunning = true;

            try
            {
                Run(token);
            }
            finally
            {
                this.IsRunning = false;
            }
        }
    }
}
