using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.CommandModules
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
