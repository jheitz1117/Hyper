using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents the primary key for a database table.
    /// </summary>
    public interface IDbForeignKeyScriptSource
    {
        /// <summary>
        /// The name of the foreign key.
        /// </summary>
        string ForeignKeyName { get; set; }

        /// <summary>
        /// The name of the table containing the primary key pointed to by this <see cref="IDbForeignKeyScriptSource"/> object.
        /// </summary>
        string ReferencedTableName { get; set; }

        /// <summary>
        /// The <see cref="IDbTableScriptSource"/> object to which this <see cref="IDbForeignKeyScriptSource"/> object belongs.
        /// </summary>
        IDbTableScriptSource TableSource { get; set; }

        /// <summary>
        /// The collection of <see cref="IDbForeignKeyMapping"/> objects representing the mappings between the foreign key columns and their respective primary key columns.
        /// </summary>
        IEnumerable<IDbForeignKeyMapping> KeyColumns { get; }
    }

    /// <summary>
    /// Describes a class which represents a mapping from a foreign key column to a primary key column
    /// </summary>
    public interface IDbForeignKeyMapping
    {
        /// <summary>
        /// The name of the foreign key column.
        /// </summary>
        string ForeignKeyColumnName { get; set; }

        /// <summary>
        /// The name of the primary key column.
        /// </summary>
        string ReferencedColumnName { get; set; }
    }

    /// <summary>
    /// The default implementation of <see cref="IDbForeignKeyMapping"/>.
    /// </summary>
    public class DbForeignKeyMapping : IDbForeignKeyMapping
    {
        /// <summary>
        /// The name of the foreign key column.
        /// </summary>
        public string ForeignKeyColumnName { get; set; }

        /// <summary>
        /// The name of the primary key column.
        /// </summary>
        public string ReferencedColumnName { get; set; }
    }
}
