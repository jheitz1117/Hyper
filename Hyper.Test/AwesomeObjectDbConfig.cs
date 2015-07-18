using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using Hyper.Db;

namespace Hyper.Test
{
    public class AwesomeObjectDbConfig : IDbCustomConfiguration
    {
        private readonly XNamespace _ns = "http://www.test.com/TestCustomDbXmlSchema";
        private readonly List<AwesomeObject> _awesomeObjects = new List<AwesomeObject>();

        public void Deserialize(XElement parent, IDictionary<string, Type> scriptWriterTypes)
        {
            _awesomeObjects.Clear();

            var awesomeObjectParent = parent.Element(_ns + "awesomeObjects");
            if (awesomeObjectParent == null)
                return;

            foreach (var awesomeObjectElement in awesomeObjectParent.Elements(_ns + "awesomeObject"))
            {
                var awesomeObject = new AwesomeObject
                {
                    Name = GetRequiredAttribute(awesomeObjectElement, "name"),
                    Property1 = GetRequiredAttribute(awesomeObjectElement, "property1"),
                };

                awesomeObject.ScriptWriter = new AwesomeObjectWriter()
                {
                    Source = awesomeObject
                };

                _awesomeObjects.Add(awesomeObject);
            }
        }

        public IEnumerable<IDbObject> GetDbObjects()
        {
            return _awesomeObjects;
        }

        private static string GetRequiredAttribute(XElement parent, string attributeName)
        {
            if (parent.Attribute(attributeName) == null)
            { throw new XmlSchemaValidationException("Attribute '" + attributeName + "' is required on element '" + parent.Name + "'."); }

            return parent.Attribute(attributeName).Value;
        }
    }

    public class AwesomeObject : IDbObject
    {
        public string Name { get; set; }
        public string Property1 { get; set; }
        public IDbScriptWriter ScriptWriter { get; set; }
    }

    public class AwesomeObjectWriter : IDbScriptWriter
    {
        public AwesomeObject Source { get; set; }
        public void WriteDbScript(TextWriter writer)
        {
            writer.WriteLine("awesome object '{0}' has Property1 set to '{1}'!!!", this.Source.Name, this.Source.Property1);
        }
    }
}
