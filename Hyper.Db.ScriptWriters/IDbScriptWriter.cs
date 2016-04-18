using System.IO;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class capable of writing SQL script.
    /// </summary>
    public interface IDbScriptWriter
    {
        /// <summary>
        /// Writes SQL script to the specified <see cref="TextWriter"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> object to which to write SQL script.</param>
        /// <param name="table">The <see cref="IDbTable"/> object from which to pull data while writing the script.</param>
        void WriteDbScript(TextWriter writer, IDbTable table);

        /// <summary>
        /// Writes SQL script to the specified <see cref="TextWriter"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> object to which to write SQL script.</param>
        /// <param name="primaryKey">The <see cref="IDbPrimaryKey"/> object from which to pull data while writing the script.</param>
        void WriteDbScript(TextWriter writer, IDbPrimaryKey primaryKey);

        /// <summary>
        /// Writes SQL script to the specified <see cref="TextWriter"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> object to which to write SQL script.</param>
        /// <param name="foreignKey">The <see cref="IDbForeignKey"/> object from which to pull data while writing the script.</param>
        void WriteDbScript(TextWriter writer, IDbForeignKey foreignKey);
    }
}
