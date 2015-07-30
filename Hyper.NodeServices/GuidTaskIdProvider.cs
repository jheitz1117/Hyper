using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices
{
    internal sealed class GuidTaskIdProvider : TaskIdProviderBase
    {
        public override string CreateTaskId(HyperNodeMessageRequest message)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
