namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents the database schema configuration for a set of databases.
    /// </summary>
    public interface IDbSchemaCollection
    {
        /// <summary>
        /// Indicates whether this <see cref="IDbSchemaCollection"/> contains an instance of <see cref="IDbSchema"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDbSchema"/> object to find.</param>
        /// <returns></returns>
        bool ContainsSchema(string name);

        /// <summary>
        /// Returns the <see cref="IDbSchema"/> object with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDbSchema"/> object to find.</param>
        /// <returns></returns>
        IDbSchema GetSchema(string name);
    }
}
