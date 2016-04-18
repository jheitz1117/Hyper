using System.Collections.Generic;
using System.Linq;

namespace Hyper.Db.Model
{
    public class DbSchema : IDbSchema
    {
        private readonly List<IDbTable> _tables = new List<IDbTable>();

        public string Name { get; set; }
        public IEnumerable<IDbTable> Tables => _tables;
        public IEnumerable<IDbPrimaryKey> PrimaryKeys => _tables.Where(t => t.PrimaryKey != null).Select(t => t.PrimaryKey);
        public IEnumerable<IDbForeignKey> ForeignKeys => _tables.SelectMany(t => t.ForeignKeys);

        public void AddTable(IDbTable table)
        {
            _tables.Add(table);
        }
    }
}
