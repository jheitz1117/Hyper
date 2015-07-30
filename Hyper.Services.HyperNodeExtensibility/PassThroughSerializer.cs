using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class PassThroughSerializer : ICommandRequestSerializer, ICommandResponseSerializer
    {
        public string Serialize(ICommandRequest request)
        {
            return request.ToString();
        }

        public string Serialize(ICommandResponse response)
        {
            return response.ToString();
        }

        ICommandResponse ICommandResponseSerializer.Deserialize(string responseString)
        {
            return new CommandResponseString(MessageProcessStatusFlags.None, responseString);
        }

        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return new CommandRequestString(requestString);
        }
    }
}
