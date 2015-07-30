using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal sealed class GuidTaskIdProvider : ITaskIdProvider
    {
        public string CreateTaskId(HyperNodeMessageRequest message)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
