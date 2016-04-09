using System;
using System.Collections.Generic;
using Hyper.Extensibility.Versioning;

namespace Hyper.Db.Xml.ScriptSources
{
    /// <summary>
    /// Represents a deserialized table, and doubles as the default IDbTableScriptSource.
    /// Since most databases do not inherently support versioning, I've decided to
    /// implement versioning as a separate interface instead of being part of the
    /// IDbTableScriptSource interface. I didn't want other implementors of
    /// IDbTableScriptSource to have to deal with versions if they don't care about them.
    /// </summary>
    internal class HyperDbTable : IDbObject, IDbTableScriptSource, ISupportsVersioning
    {
        public IDbScriptWriter ScriptWriter { get; set; }
        
        public string Name { get; set; }
        public Version Version { get; set; }
        public readonly List<HyperDbColumn> Columns = new List<HyperDbColumn>();
        public HyperDbPrimaryKey PrimaryKey { get; set; }
        public readonly List<HyperDbForeignKey> ForeignKeys = new List<HyperDbForeignKey>();

        string IDbTableScriptSource.TableName => Name;
        IDbPrimaryKeyScriptSource IDbTableScriptSource.PrimaryKey => PrimaryKey;
        IEnumerable<IDbColumnScriptSource> IDbTableScriptSource.Columns => Columns;
        IEnumerable<IDbForeignKeyScriptSource> IDbTableScriptSource.ForeignKeys => ForeignKeys;
    }
}
