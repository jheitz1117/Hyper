using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using Hyper.Db;
using Hyper.Db.Xml;

namespace Hyper.Test
{
    public class AwesomeObjectCollection : IXmlDbObjectCollection
    {
        private readonly XNamespace _ns = "http://www.test.com/TestCustomDbXmlSchema";
        private readonly List<AwesomeObject> _awesomeObjects = new List<AwesomeObject>();

        public void Deserialize(XElement parent)
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
    }

    public class AwesomeObjectScriptWriterDecorator : IDbScriptWriter
    {
        private readonly IDbScriptWriter _underlyingWriter;

        public AwesomeObjectScriptWriterDecorator(IDbScriptWriter underlyingWriter)
        {
            _underlyingWriter = underlyingWriter;
        }

        public void WriteDbScript(TextWriter writer, IDbTable table)
        {
            _underlyingWriter.WriteDbScript(writer, table);
        }

        public void WriteDbScript(TextWriter writer, IDbPrimaryKey primaryKey)
        {
            _underlyingWriter.WriteDbScript(writer, primaryKey);
        }

        public void WriteDbScript(TextWriter writer, IDbForeignKey foreignKey)
        {
            _underlyingWriter.WriteDbScript(writer, foreignKey);
        }

        public void WriteDbScript(TextWriter writer, AwesomeObject source)
        {
            writer.WriteLine($"Awesome object '{source.Name}' has Property1 set to '{source.Property1}'!!!");
        }
    }
}
