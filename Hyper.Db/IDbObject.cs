namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a database object.
    /// </summary>
    public interface IDbObject
    {
        /// <summary>
        /// The <see cref="IDbScriptWriter" /> object responsible for writing SQL to create/update this <see cref="IDbObject"/> object.
        /// </summary>
        IDbScriptWriter ScriptWriter { get; set; }
    }
}
