using System;
using System.Collections.Generic;
using Hyper.Extensibility.Versioning;

namespace Hyper.Db.Xml.DbObjects
{
    internal class HyperDbStoredProcedure : IDbObject, ISupportsVersioning
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public List<HyperDbParameter> Parameters { get; set; }
        public string Body { get; set; }
    }
}
