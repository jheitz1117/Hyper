using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
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

        public Type GetRequestType()
        {
            return typeof(CommandRequestString);
        }

        public Type GetResponseType()
        {
            return typeof(CommandResponseString);
        }
    }
}
