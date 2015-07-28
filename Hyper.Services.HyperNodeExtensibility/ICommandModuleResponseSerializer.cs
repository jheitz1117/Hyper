namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleResponseSerializer
    {
        string Serialize(ICommandModuleResponse response);
        ICommandModuleResponse Deserialize(string responseString);
    }
}
