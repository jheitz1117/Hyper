namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleResponseSerializer
    {
        string Serialize(ICommandModuleResponse responseObject);
        ICommandModuleResponse Deserialize(string responseString);
    }
}
