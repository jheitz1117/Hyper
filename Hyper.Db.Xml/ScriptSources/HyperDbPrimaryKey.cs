using System.Collections.Generic;

namespace Hyper.Db.Xml.ScriptSources
{
    internal class HyperDbPrimaryKey : IDbObject, IDbPrimaryKeyScriptSource
    {
        public IDbScriptWriter ScriptWriter { get; set; }

        public string PrimaryKeyName { get; set; }
        public IDbTableScriptSource TableSource { get; set; }
        public readonly List<string> KeyColumns = new List<string>();

        IEnumerable<string> IDbPrimaryKeyScriptSource.KeyColumns { get { return KeyColumns; } }
    }
}
