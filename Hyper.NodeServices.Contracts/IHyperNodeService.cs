using System.ServiceModel;

namespace Hyper.NodeServices.Contracts
{
    [ServiceContract]
    public interface IHyperNodeService
    {
        [OperationContract]
        HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message);
    }
}
