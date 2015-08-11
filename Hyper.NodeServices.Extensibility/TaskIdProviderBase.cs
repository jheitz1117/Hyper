namespace Hyper.NodeServices.Extensibility
{
    public abstract class TaskIdProviderBase : ITaskIdProvider
    {
        public virtual void Initialize() { }
        public abstract string CreateTaskId(IHyperNodeMessageContext context);
    }
}
