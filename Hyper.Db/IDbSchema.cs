using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents the schema for an entire database.
    /// </summary>
    public interface IDbSchema
    {
        /// <summary>
        /// The name of the database described by this <see cref="IDbSchema"/> object.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IDbObject"/> objects belonging to the database described by this <see cref="IDbSchema"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbObject> GetDbObjects();
    }
}
