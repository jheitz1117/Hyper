using System.Collections.Generic;

namespace Hyper.Db.Model
{
    public class DbForeignKey : IDbForeignKey
    {
        public string Name { get; set; }
        public string ReferencedTableName { get; set; }
        public IDbTable TableSource { get; set; }
        public readonly List<DbForeignKeyMapping> KeyColumns = new List<DbForeignKeyMapping>();

        IEnumerable<IDbForeignKeyMapping> IDbForeignKey.KeyColumns => KeyColumns;
    }
}
