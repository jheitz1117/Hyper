using System.Collections.Generic;

namespace Hyper.Db.Xml.ScriptSources
{
    internal class HyperDbForeignKey : IDbObject, IDbForeignKeyScriptSource
    {
        public IDbScriptWriter ScriptWriter { get; set; }

        public string ForeignKeyName { get; set; }
        public string ReferencedTableName { get; set; }
        public IDbTableScriptSource TableSource { get; set; }
        public readonly List<DbForeignKeyMapping> KeyColumns = new List<DbForeignKeyMapping>();

        IEnumerable<IDbForeignKeyMapping> IDbForeignKeyScriptSource.KeyColumns
        {
            get
            {
                return KeyColumns;
            }
        }
    }
}
