namespace Hyper.NodeServices.Extensibility
{
    public interface ITaskIdProvider
    {
        void Initialize();
        string CreateTaskId(IHyperNodeMessageContext context);
    }
}
