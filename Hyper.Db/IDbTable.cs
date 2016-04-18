using System.Collections.Generic;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which supplies data to a <see cref="IDbTableScriptWriter"/> object.
    /// </summary>
    public interface IDbTable : IDbObject
    {
        /// <summary>
        /// Gets a collection of <see cref="IDbColumn"/> objects that make up the table described by this <see cref="IDbTable"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbColumn> Columns { get; }

        /// <summary>
        /// Gets the <see cref="IDbPrimaryKey"/> object for the table described by this <see cref="IDbTable"/> object.
        /// </summary>
        /// <returns></returns>
        IDbPrimaryKey PrimaryKey { get; }

        /// <summary>
        /// Gets a collection of <see cref="IDbForeignKey"/> objects for the table described by this <see cref="IDbTable"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbForeignKey> ForeignKeys { get; }
    }
}
