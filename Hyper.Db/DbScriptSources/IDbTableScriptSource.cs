using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which supplies data to a <see cref="IDbTableScriptWriter"/> object.
    /// </summary>
    public interface IDbTableScriptSource
    {
        /// <summary>
        /// Gets the name of the table described by this <see cref="IDbTableScriptSource"/> object.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Gets a collection of <see cref="IDbColumnScriptSource"/> objects that make up the table described by this <see cref="IDbTableScriptSource"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbColumnScriptSource> Columns { get; }

        /// <summary>
        /// Gets the <see cref="IDbPrimaryKeyScriptSource"/> object for the table described by this <see cref="IDbTableScriptSource"/> object.
        /// </summary>
        /// <returns></returns>
        IDbPrimaryKeyScriptSource PrimaryKey { get; }

        /// <summary>
        /// Gets a collection of <see cref="IDbForeignKeyScriptSource"/> objects for the table described by this <see cref="IDbTableScriptSource"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbForeignKeyScriptSource> ForeignKeys { get; }
    }
}
