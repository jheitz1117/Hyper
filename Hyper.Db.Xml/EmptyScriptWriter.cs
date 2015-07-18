using System.IO;

namespace Hyper.Db.Xml.ScriptWriters
{
    /// <summary>
    /// This <see cref="EmptyScriptWriter"/> is just a default placeholder for when no <see cref="IDbScriptWriter"/> is specified in the XML.
    /// It doesn't write anything out at all.
    /// </summary>
    internal class EmptyScriptWriter : IDbTableScriptWriter, IDbPrimaryKeyScriptWriter, IDbForeignKeyScriptWriter
    {
        IDbTableScriptSource IDbTableScriptWriter.Source { get; set; }
        IDbPrimaryKeyScriptSource IDbPrimaryKeyScriptWriter.Source { get; set; }
        IDbForeignKeyScriptSource IDbForeignKeyScriptWriter.Source { get; set; }

        public void WriteDbScript(TextWriter writer) { }
    }
}
