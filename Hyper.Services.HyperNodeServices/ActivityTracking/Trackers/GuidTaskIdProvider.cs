using System;
using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeServices
{
    public sealed class GuidTaskIdProvider : ITaskIdProvider
    {
        public string CreateTaskId(HyperNodeMessageRequest message)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
