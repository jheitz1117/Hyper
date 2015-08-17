namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Defines a node in a <see cref="IPath{T}"/> tree
    /// </summary>
    public interface IVertex
    {
        string Key { get; set; }
    }
}
