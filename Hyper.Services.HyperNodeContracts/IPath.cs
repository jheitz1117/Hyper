using System.Collections.Generic;

namespace Hyper.Services.HyperNodeContracts
{
    public interface IPath<out TVertex> where TVertex : IVertex
    {
        IEnumerable<TVertex> GetChildren(string parentKey);
    }
}
