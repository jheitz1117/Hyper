namespace Hyper.NodeServices.Contracts.Extensibility
{
    public interface ICommandResponseSerializer
    {
        string Serialize(ICommandResponse response);
        ICommandResponse Deserialize(string responseString);
    }
}
