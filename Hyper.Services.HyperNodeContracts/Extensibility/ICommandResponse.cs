namespace Hyper.Services.HyperNodeContracts.Extensibility
{
    public interface ICommandResponse
    {
        MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
