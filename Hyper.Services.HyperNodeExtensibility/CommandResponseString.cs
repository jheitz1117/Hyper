using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public sealed class CommandResponseString : CommandResponse
    {
        public string ResponseString { get; set; }

        public CommandResponseString(MessageProcessStatusFlags statusFlags)
            : base(statusFlags) { }

        public CommandResponseString(MessageProcessStatusFlags statusFlags, string responseString)
            : this(statusFlags)
        {
            this.ResponseString = responseString;
        }

        public override string ToString()
        {
            return this.ResponseString;
        }
    }

}
