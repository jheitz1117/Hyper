using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a foreign key for a database table.
    /// </summary>
    public interface IDbForeignKey : IDbObject
    {
        /// <summary>
        /// The name of the table containing the primary key pointed to by this <see cref="IDbForeignKey"/> object.
        /// </summary>
        string ReferencedTableName { get; set; }

        /// <summary>
        /// The <see cref="IDbTable"/> object to which this <see cref="IDbForeignKey"/> object belongs.
        /// </summary>
        IDbTable TableSource { get; set; }

        /// <summary>
        /// The collection of <see cref="IDbForeignKeyMapping"/> objects representing the mappings between the foreign key columns and their respective primary key columns.
        /// </summary>
        IEnumerable<IDbForeignKeyMapping> KeyColumns { get; }
    }
}
