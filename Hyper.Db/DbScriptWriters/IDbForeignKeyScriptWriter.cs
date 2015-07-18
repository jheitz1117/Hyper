namespace Hyper.Db
{
    /// <summary>
    /// Describes a class capable of writing SQL script for updating foreign keys for database tables.
    /// </summary>
    public interface IDbForeignKeyScriptWriter : IDbScriptWriter
    {
        /// <summary>
        /// The <see cref="IDbForeignKeyScriptSource"/> object from which to pull data while writing the script.
        /// </summary>
        IDbForeignKeyScriptSource Source { get; set; }
    }
}
