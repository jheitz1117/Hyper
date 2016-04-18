using System;
using Hyper.Db.Model;
using Hyper.Extensibility.Versioning;

namespace Hyper.Db.Xml
{
    public class XmlDbSchema : DbSchema, ISupportsVersioning
    {
        public Version Version { get; set; }
        public IXmlDbObjectCollection XmlDbObjects { get; set; }
    }
}
