using System.IO;

namespace Hyper.Db.ScriptWriters.Sql
{
    public abstract class SqlScriptWriter : IDbScriptWriter
    {
        public string DefaultSchemaName { get; set; } = "dbo";

        public abstract void WriteDbScript(TextWriter writer);
    }
}
