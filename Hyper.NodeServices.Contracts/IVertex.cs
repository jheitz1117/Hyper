namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Defines a node in a <see cref="IPath{T}"/> tree.
    /// </summary>
    public interface IVertex
    {
        /// <summary>
        /// The unique key of the <see cref="IVertex"/>.
        /// </summary>
        string Key { get; set; }
    }
}
