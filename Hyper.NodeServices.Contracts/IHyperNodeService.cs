﻿using System.ServiceModel;
using System.Threading.Tasks;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Processes <see cref="HyperNodeMessageRequest"/> objects and returns <see cref="HyperNodeMessageResponse"/> objects.
    /// </summary>
    [ServiceContract]
    public interface IHyperNodeService
    {
        /// <summary>
        /// Processes and/or forwards the specified message.
        /// </summary>
        /// <param name="message">The <see cref="HyperNodeMessageRequest"/> object to process.</param>
        /// <returns></returns>
        [OperationContract]
        Task<HyperNodeMessageResponse> ProcessMessageAsync(HyperNodeMessageRequest message);
    }
}
