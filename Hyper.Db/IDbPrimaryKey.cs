using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents the primary key for a database table.
    /// </summary>
    public interface IDbPrimaryKey : IDbObject
    {
        /// <summary>
        /// The <see cref="IDbTable"/> object to which this <see cref="IDbPrimaryKey"/> object belongs.
        /// </summary>
        IDbTable TableSource { get; set; }

        /// <summary>
        /// The list of columns that make up this <see cref="IDbPrimaryKey"/> object.
        /// </summary>
        IEnumerable<string> KeyColumns { get; }
    }
}