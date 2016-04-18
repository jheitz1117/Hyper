namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a generic database object.
    /// </summary>
    public interface IDbObject
    {
        /// <summary>
        /// The name of the database object.
        /// </summary>
        string Name { get; set; }
    }
}
