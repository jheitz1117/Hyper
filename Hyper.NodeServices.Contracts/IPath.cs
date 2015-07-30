using System.Collections.Generic;

namespace Hyper.NodeServices.Contracts
{
    public interface IPath<out TVertex> where TVertex : IVertex
    {
        IEnumerable<TVertex> GetChildren(string parentKey);
    }
}
