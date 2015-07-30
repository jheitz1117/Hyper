namespace Hyper.Services.HyperNodeContracts.Extensibility
{
    public interface ICommandRequestSerializer
    {
        string Serialize(ICommandRequest request);
        ICommandRequest Deserialize(string requestString);
    }
}
