using System.IO;

namespace Hyper.Db.ScriptWriters.Sql
{
    public abstract class SqlScriptWriter : IDbScriptWriter
    {
        public string DefaultSchemaName
        {
            get { return _defaultSchemaName; }
            set { _defaultSchemaName = value; }
        } private string _defaultSchemaName = "dbo";

        public abstract void WriteDbScript(TextWriter writer);
    }
}
