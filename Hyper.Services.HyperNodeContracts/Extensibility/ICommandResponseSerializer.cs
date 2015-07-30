namespace Hyper.Services.HyperNodeContracts.Extensibility
{
    public interface ICommandResponseSerializer
    {
        string Serialize(ICommandResponse response);
        ICommandResponse Deserialize(string responseString);
    }
}
