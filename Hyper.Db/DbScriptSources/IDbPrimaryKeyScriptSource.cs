using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents the primary key for a database table.
    /// </summary>
    public interface IDbPrimaryKeyScriptSource
    {
        /// <summary>
        /// The name of the primary key.
        /// </summary>
        string PrimaryKeyName { get; set; }

        /// <summary>
        /// The <see cref="IDbTableScriptSource"/> object to which this <see cref="IDbPrimaryKeyScriptSource"/> object belongs.
        /// </summary>
        IDbTableScriptSource TableSource { get; set; }

        /// <summary>
        /// The list of columns that make up this <see cref="IDbPrimaryKeyScriptSource"/> object.
        /// </summary>
        IEnumerable<string> KeyColumns { get; }
    }
}