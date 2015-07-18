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
        void WriteDbScript(TextWriter writer);
    }
}
