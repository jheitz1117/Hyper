using System.ServiceModel;

namespace Hyper.Services.HyperNodeContracts
{
    [ServiceContract]
    public interface IHyperNodeService
    {
        [OperationContract]
        HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message);
    }
}
