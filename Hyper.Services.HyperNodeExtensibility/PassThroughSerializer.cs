namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class PassThroughSerializer : ICommandModuleRequestSerializer, ICommandModuleResponseSerializer
    {
        public string Serialize(ICommandModuleRequest requestObject)
        {
            return requestObject.ToString();
        }

        public string Serialize(ICommandModuleResponse responseObject)
        {
            return responseObject.ToString();
        }

        ICommandModuleResponse ICommandModuleResponseSerializer.Deserialize(string responseString)
        {
            return new CommandResponseString(responseString);
        }

        ICommandModuleRequest ICommandModuleRequestSerializer.Deserialize(string requestString)
        {
            return new CommandRequestString(requestString);
        }
    }
}
