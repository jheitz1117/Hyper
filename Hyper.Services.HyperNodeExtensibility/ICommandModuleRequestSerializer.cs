namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleRequestSerializer
    {
        string Serialize(ICommandModuleRequest requestObject);
        ICommandModuleRequest Deserialize(string requestString);
    }
}
