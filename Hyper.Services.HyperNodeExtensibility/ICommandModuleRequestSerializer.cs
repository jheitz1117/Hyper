namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleRequestSerializer
    {
        string Serialize(ICommandModuleRequest request);
        ICommandModuleRequest Deserialize(string requestString);
    }
}
