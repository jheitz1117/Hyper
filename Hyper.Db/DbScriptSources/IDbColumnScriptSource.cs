namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a column for a database table.
    /// </summary>
    public interface IDbColumnScriptSource
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The data type of the column. This property is a free-form string by design to provide the most flexibility across all database systems.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The maximum length which values in the column can take.
        /// </summary>
        int? MaxLength { get; set; }

        /// <summary>
        /// The number of decimal places in the column. Only applies to floating decimal types.
        /// </summary>
        int? Decimals { get; set; }

        /// <summary>
        /// Indicates whether the column is nullable. If this property is not specified, the database default behavior is used.
        /// </summary>
        bool? IsNullable { get; set; }

        /// <summary>
        /// Specifies the default value of the column.
        /// </summary>
        string DefaultValue { get; set; }
    }
}
