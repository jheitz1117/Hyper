using System;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.TaskIdProviders
{
    internal sealed class GuidTaskIdProvider : TaskIdProviderBase
    {
        public override string CreateTaskId(IReadOnlyHyperNodeMessageInfo message)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
