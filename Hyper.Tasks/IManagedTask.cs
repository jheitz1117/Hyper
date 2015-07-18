using System.Threading;

namespace Hyper.Tasks
{
    public interface IManagedTask
    {
        IManagedTaskExceptionHandler ExceptionHandler { get; set; }
        void ExecuteTask(CancellationToken token);
    }
}
