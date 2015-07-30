namespace Hyper.NodeServices.Contracts.Extensibility
{
    public interface ICommandResponse
    {
        MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
