using System.Collections.Generic;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Provides methods to traverse a path tree composed of <see cref="IVertex"/> objects.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public interface IPath<out TVertex> where TVertex : IVertex
    {
        /// <summary>
        /// Gets the immediate children of the <see cref="IVertex"/> with the specified key.
        /// </summary>
        /// <param name="parentKey">The key of the parent for which to find child nodes.</param>
        /// <returns></returns>
        IEnumerable<TVertex> GetChildren(string parentKey);
    }
}
