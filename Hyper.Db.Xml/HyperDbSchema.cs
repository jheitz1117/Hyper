using System;
using System.Collections.Generic;
using System.Linq;
using Hyper.Db.Xml.ScriptSources;
using Hyper.Versioning;

namespace Hyper.Db.Xml
{
    internal class HyperDbSchema : IDbSchema, ISupportsVersioning
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public IDbCustomConfiguration DbCustomConfig { get; set; }
        public readonly List<HyperDbTable> Tables = new List<HyperDbTable>();
        public readonly Dictionary<string, HyperDbScript> Scripts = new Dictionary<string, HyperDbScript>();

        public IEnumerable<IDbObject> GetDbObjects()
        {
            var dbObjects = new List<IDbObject>();

            dbObjects.AddRange(Tables);
            dbObjects.AddRange(Tables.Where(t => t.PrimaryKey != null).Select(t => t.PrimaryKey));
            foreach (var table in Tables)
            {
                dbObjects.AddRange(table.ForeignKeys);
            }

            if (this.DbCustomConfig != null)
            {
                dbObjects.AddRange(
                    this.DbCustomConfig.GetDbObjects()
                );
            }

            return dbObjects;
        }
    }
}
