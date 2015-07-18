namespace Hyper.Db
{
    /// <summary>
    /// Describes a class capable of writing SQL script for updating primary keys for database tables.
    /// </summary>
    public interface IDbPrimaryKeyScriptWriter : IDbScriptWriter
    {
        /// <summary>
        /// The <see cref="IDbPrimaryKeyScriptSource"/> object from which to pull data while writing the script.
        /// </summary>
        IDbPrimaryKeyScriptSource Source { get; set; }
    }
}
