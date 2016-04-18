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
        /// The tables contained in this <see cref="IDbSchema"/>
        /// </summary>
        IEnumerable<IDbTable> Tables { get; }

        /// <summary>
        /// The primary keys contained in this <see cref="IDbSchema"/>
        /// </summary>
        IEnumerable<IDbPrimaryKey> PrimaryKeys { get; }

        /// <summary>
        /// The foreign keys contained in this <see cref="IDbSchema"/>
        /// </summary>
        IEnumerable<IDbForeignKey> ForeignKeys { get; }
    }
}
