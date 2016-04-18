namespace Hyper.Db.Model
{
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
