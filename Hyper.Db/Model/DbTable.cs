using System.Collections.Generic;

namespace Hyper.Db.Model
{
    public class DbTable : IDbTable
    {
        public string Name { get; set; }
        public readonly List<DbColumn> Columns = new List<DbColumn>();
        public DbPrimaryKey PrimaryKey { get; set; }
        public readonly List<DbForeignKey> ForeignKeys = new List<DbForeignKey>();

        IDbPrimaryKey IDbTable.PrimaryKey => PrimaryKey;
        IEnumerable<IDbColumn> IDbTable.Columns => Columns;
        IEnumerable<IDbForeignKey> IDbTable.ForeignKeys => ForeignKeys;
    }
}
