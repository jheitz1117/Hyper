using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    [DataContract]
    public class HyperNodePath : IPath<HyperNodeVertex>
    {
        [DataMember]
        public ConcurrentDictionary<string, List<HyperNodeVertex>> PathTree { get; set; }

        public HyperNodePath()
        {
            this.PathTree = new ConcurrentDictionary<string, List<HyperNodeVertex>>();
        }

        public IEnumerable<HyperNodeVertex> GetChildren(string parentKey)
        {
            List<HyperNodeVertex> vertices;
            this.PathTree.TryGetValue(parentKey, out vertices);

            return vertices ?? new List<HyperNodeVertex>();
        }
    }
}
