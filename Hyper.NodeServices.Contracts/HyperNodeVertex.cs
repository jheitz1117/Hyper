using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Concrete implementation of <see cref="IVertex"/> for HyperNode network paths.
    /// </summary>
    [DataContract]
    public class HyperNodeVertex : IVertex
    {
        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/> represented by this instance.
        /// </summary>
        [DataMember]
        public string Key { get; set; }
    }
}
