using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Configuration;

namespace Hyper.Services.HyperNodeContracts
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

            vertices = vertices ?? new List<HyperNodeVertex>();

            // Check to see if the path specified any child nodes that don't have endpoints defined in the app.config
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModelGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
            if (serviceModelGroup == null)
            {
                // No endpoints exist because no service model section exists

                // TODO: Report (via activity tracker) that configuration doesn't contain a service model section so forwarding is disabled

                // For now, just clear the list
                vertices.Clear();
            }
            else
            {
                // TODO: Report that child with name XXX doesn't exist in the configuration, therefore removing it from list of children specified in path tree.
                // Might consider also saying that the user should "consider rebuilding the HyperNode adjacency list" to update it to match the actual network, or else
                // update the configuration

                // Remove all vertices that don't have endpoint configurations in the app.config
                vertices.RemoveAll(
                    v => !serviceModelGroup.Client.Endpoints
                        .Cast<ChannelEndpointElement>()
                        .Any(e => e.Contract == typeof(IHyperNodeService).FullName &&
                            e.Name == v.Key));
            }

            return vertices;
        }
    }
}
