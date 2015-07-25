using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class CommandResponseString : ICommandModuleResponse
    {
        public string ResponseString { get; set; }
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        public CommandResponseString(string responseString)
        {
            this.ResponseString = responseString;
        }

        public CommandResponseString(string responseString, MessageProcessStatusFlags statusFlags)
            : this(responseString)
        {
            this.ProcessStatusFlags = statusFlags;
        }

        public override string ToString()
        {
            return this.ResponseString;
        }
    }

}
