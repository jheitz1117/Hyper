namespace Hyper.Db
{
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
}
