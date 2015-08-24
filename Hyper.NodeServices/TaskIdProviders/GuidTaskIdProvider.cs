using System;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.TaskIdProviders
{
    internal sealed class GuidTaskIdProvider : TaskIdProviderBase
    {
        public override string CreateTaskId(IHyperNodeMessageContext context)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
