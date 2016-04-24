using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace NodeModuleTest.Contracts.DbUpdate
{
    [DataContract]
    public class DbUpdateRequest : ICommandRequest
    {
        /// <summary>
        /// Allows the user to only update certain databases. If this property is null or references an empty array, then all databases
        /// for which the target <see cref="IHyperNodeService"/> is responsible will be updated.
        /// </summary>
        [DataMember]
        public string[] TargetDatabases { get; set; }
    }
}
