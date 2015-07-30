namespace Hyper.NodeServices.Contracts.Extensibility
{
    public interface ICommandRequestSerializer
    {
        string Serialize(ICommandRequest request);
        ICommandRequest Deserialize(string requestString);
    }
}
