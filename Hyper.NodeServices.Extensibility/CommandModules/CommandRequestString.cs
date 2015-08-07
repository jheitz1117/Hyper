using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public sealed class CommandRequestString : ICommandRequest
    {
        public string RequestString { get; set; }

        public CommandRequestString(string requestString)
        {
            this.RequestString = requestString;
        }

        public override string ToString()
        {
            return this.RequestString;
        }
    }
}
