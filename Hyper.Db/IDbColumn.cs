namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a column for a database table.
    /// </summary>
    public interface IDbColumn : IDbObject
    {
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

        /// <summary>
        /// Indicates that the column is an identity column.
        /// </summary>
        bool? IsIdentity { get; set; }

        /// <summary>
        /// Specifies the seed to use if the column is an identity column.
        /// </summary>
        long? Seed { get; set; }

        /// <summary>
        /// Specifies the increment to use if the column is an identity column.
        /// </summary>
        int? Increment { get; set; }
    }
}
