using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public class CommandResponse : ICommandModuleResponse
    {
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        public CommandResponse(MessageProcessStatusFlags statusFlags)
        {
            this.ProcessStatusFlags = statusFlags;
        }

        public override string ToString()
        {
            return this.ProcessStatusFlags.ToString();
        }
    }

}
