using System;
using Hyper.Db.Model;
using Hyper.Extensibility.Versioning;

namespace Hyper.Db.Xml.DbObjects
{
    /// <summary>
    /// Represents a deserialized table. Since most databases do not inherently support
    /// versioning, I've decided to implement versioning as a separate interface instead
    /// of being part of the <see cref="IDbTable"/> interface. I didn't want other
    /// implementors of <see cref="IDbTable"/> to have to deal with versions if they
    /// don't care about them.
    /// </summary>
    internal class XmlDbTable : DbTable, ISupportsVersioning
    {
        public Version Version { get; set; }
    }
}
