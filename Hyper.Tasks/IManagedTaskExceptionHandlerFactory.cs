namespace Hyper.Tasks
{
    public interface IManagedTaskExceptionHandlerFactory
    {
        IManagedTaskExceptionHandler Create(params object[] args);
    }
}
