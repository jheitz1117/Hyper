using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Concrete implementation of <see cref="IPath{T}"/> for HyperNode network path trees.
    /// </summary>
    [DataContract]
    public class HyperNodePath : IPath<HyperNodeVertex>
    {
        /// <summary>
        /// A dictionary containing the full network path for the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<string, List<HyperNodeVertex>> PathTree { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodePath"/>.
        /// </summary>
        public HyperNodePath()
        {
            PathTree = new ConcurrentDictionary<string, List<HyperNodeVertex>>();
        }

        /// <summary>
        /// Gets the immediate children of the <see cref="IHyperNodeService"/> with the specified name.
        /// </summary>
        /// <param name="parentKey">The name of the parent for which to find child nodes.</param>
        /// <returns></returns>
        public IEnumerable<HyperNodeVertex> GetChildren(string parentKey)
        {
            List<HyperNodeVertex> vertices;
            PathTree.TryGetValue(parentKey, out vertices);

            return vertices ?? new List<HyperNodeVertex>();
        }
    }
}
