using System.Collections.Generic;

namespace Hyper.Db.Model
{
    public class DbPrimaryKey : IDbPrimaryKey
    {
        public string Name { get; set; }
        public IDbTable TableSource { get; set; }
        public readonly List<string> KeyColumns = new List<string>();

        IEnumerable<string> IDbPrimaryKey.KeyColumns => KeyColumns;
    }
}
