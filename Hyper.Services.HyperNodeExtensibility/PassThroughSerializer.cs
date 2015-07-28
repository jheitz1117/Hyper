using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class PassThroughSerializer : ICommandModuleRequestSerializer, ICommandModuleResponseSerializer
    {
        public string Serialize(ICommandModuleRequest request)
        {
            return request.ToString();
        }

        public string Serialize(ICommandModuleResponse response)
        {
            return response.ToString();
        }

        ICommandModuleResponse ICommandModuleResponseSerializer.Deserialize(string responseString)
        {
            return new CommandResponseString(MessageProcessStatusFlags.None, responseString);
        }

        ICommandModuleRequest ICommandModuleRequestSerializer.Deserialize(string requestString)
        {
            return new CommandRequestString(requestString);
        }
    }
}
