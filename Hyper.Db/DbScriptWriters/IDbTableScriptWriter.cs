namespace Hyper.Db
{
    /// <summary>
    /// Describes a class capable of writing SQL script for updating database tables.
    /// </summary>
    public interface IDbTableScriptWriter : IDbScriptWriter
    {
        /// <summary>
        /// The <see cref="IDbTableScriptSource"/> object from which to pull data while writing the script.
        /// </summary>
        IDbTableScriptSource Source { get; set; }
    }
}
