using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Hyper.Db.Xml
{
    /// <summary>
    /// Class responsible for validating database XML configuration files and translating them into SQL scripts.
    /// </summary>
    public class HyperDbXmlScriptProvider : IDbScriptProvider
    {
        private XmlSchemaSet _dbXmlSchemaSet = new XmlSchemaSet();
        private IDbSchemaConfiguration _dbConfig;
        private static readonly object Lock = new object();

        /// <summary>
        /// Default database XML schema used to validate database schema files.
        /// </summary>
        private static XmlSchema HyperDbXmlSchema
        {
            get
            {
                if (_hyperDbXmlSchema == null)
                {
                    lock (Lock)
                    {
                        if (_hyperDbXmlSchema == null)
                        {
                            var assembly = typeof(HyperDbXmlScriptProvider).Assembly;
                            using (var xsdStream = assembly.GetManifestResourceStream("Hyper.Db.Xml.HyperDbXmlSchema.xsd"))
                            {
                                if (xsdStream == null)
                                { throw new InvalidOperationException("Unable to find HyperDbXmlSchema.xsd in " + assembly); }

                                // Specifying null for the event handler causes an XmlSchemaException to be raised if any validation errors occur.
                                _hyperDbXmlSchema = XmlSchema.Read(xsdStream, null);
                            }
                        }
                    }
                }

                return _hyperDbXmlSchema;
            }
        } private static XmlSchema _hyperDbXmlSchema;

        /// <summary>
        /// Creates a new instance of <see cref="HyperDbXmlScriptProvider"/> using the default database XML schema
        /// </summary>
        public HyperDbXmlScriptProvider()
        {
            // Make sure we always start with our HyperDbXmlSchema.xsd schema loaded up
            AddDbXmlSchema(HyperDbXmlSchema);
        }

        /// <summary>
        /// Adds the specified <see cref="XmlSchema"/> object to the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="schema"><see cref="XmlSchema"/> object to add.</param>
        public void AddDbXmlSchema(XmlSchema schema)
        {
            _dbXmlSchemaSet.Add(schema);
        }

        /// <summary>
        /// Adds the specified XML schema to the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="targetNamespace">The schema targetNamespace property, or null to use the targetNamespace specified in the schema.</param>
        /// <param name="schemaDocument">The <see cref="XmlReader"/> object containing the schema to add.</param>
        public void AddDbXmlSchema(string targetNamespace, XmlReader schemaDocument)
        {
            _dbXmlSchemaSet.Add(targetNamespace, schemaDocument);
        }

        /// <summary>
        /// Adds the specified XML schema to the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="targetNamespace">The schema targetNamespace property, or null to use the targetNamespace specified in the schema.</param>
        /// <param name="schemaUri">The URL that specifies the schema to load.</param>
        public void AddDbXmlSchema(string targetNamespace, string schemaUri)
        {
            _dbXmlSchemaSet.Add(targetNamespace, schemaUri);
        }

        /// <summary>
        /// Adds all the XML schemas contained in the specified <see cref="XmlSchemaSet"/> object to the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="schemas"><see cref="XmlSchemaSet"/> object from which to add schemas.</param>
        public void AddDbXmlSchema(XmlSchemaSet schemas)
        {
            _dbXmlSchemaSet.Add(schemas);
        }

        /// <summary>
        /// Removes the specified <see cref="XmlSchema"/> object from the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="schema">The <see cref="XmlSchema"/> object to remove.</param>
        public void RemoveDbXmlSchema(XmlSchema schema)
        {
            _dbXmlSchemaSet.Remove(schema);
        }

        /// <summary>
        /// Removes the specified <see cref="XmlSchema"/> object and all the schemas it imports from the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        /// <param name="schema">The <see cref="XmlSchema"/> object to remove.</param>
        public void RemoveDbXmlSchemaRecursive(XmlSchema schema)
        {
            _dbXmlSchemaSet.RemoveRecursive(schema);
        }

        /// <summary>
        /// Removes all the <see cref="XmlSchema"/> objects from the <see cref="XmlSchemaSet"/> used to validate database schema files.
        /// </summary>
        public void ClearDbXmlSchemas()
        {
            _dbXmlSchemaSet = new XmlSchemaSet();
        }

        /// <summary>
        /// Loads the specified XML document and optionally validates it. Then the document is deserialized using the default <see cref="IDbSchemaConfiguration"/> object.
        /// </summary>
        /// <param name="dbXmlDocument"><see cref="XDocument"/> object containing the database schema to load.</param>
        /// <param name="validateSchema">Indicates whether or not to validate the dbXmlDocument parameter before attempting to deserialize it.</param>
        public void LoadDbXmlDocument(XDocument dbXmlDocument, bool validateSchema)
        {
            LoadDbXmlDocument(dbXmlDocument, validateSchema, new HyperDbSchemaConfiguration(HyperDbXmlSchema.TargetNamespace));
        }

        /// <summary>
        /// Loads the specified XML document and optionally validates it. Then the document is deserialized using the specified <see cref="IDbSchemaConfiguration"/> object.
        /// </summary>
        /// <param name="dbXmlDocument"><see cref="XDocument"/> object containing the database schema to load.</param>
        /// <param name="validateSchema">Indicates whether or not to validate the dbXmlDocument parameter before attempting to deserialize it.</param>
        /// <param name="config"><see cref="IDbSchemaConfiguration"/> object to use for deserialization.</param>
        public void LoadDbXmlDocument(XDocument dbXmlDocument, bool validateSchema, IDbSchemaConfiguration config)
        {
            if (dbXmlDocument == null)
            { throw new ArgumentNullException("dbXmlDocument"); }
            if (config == null)
            { throw new ArgumentNullException("config"); }

            // Now optionally validate against our database XML schemas
            if (validateSchema)
            {
                var builder = new StringBuilder();
                ValidateDbXmlDocument(dbXmlDocument, (sender, args) =>
                {
                    builder.AppendLine(args.Message);
                }, false);

                // Check for validation errors before proceeding to deserialize the database schema document
                if (builder.Length > 0)
                { throw new XmlSchemaValidationException(builder.ToString()); }
            }

            _dbConfig = config;
            _dbConfig.Deserialize(dbXmlDocument);
        }

        /// <summary>
        /// Validates the specified database schema document.
        /// </summary>
        /// <param name="dbXmlDocument"><see cref="XDocument"/> object containing the database schema to validate.</param>
        /// <param name="validationEventHandler">Callback for validation errors. If this is null, an exception is thrown if any validation errors occur.</param>
        /// <param name="addSchemaInfo">Indicates whether to populate the post-schema-validation infoset (PSVI).</param>
        public void ValidateDbXmlDocument(XDocument dbXmlDocument, ValidationEventHandler validationEventHandler, bool addSchemaInfo)
        {
            if (dbXmlDocument == null)
            { throw new ArgumentNullException("dbXmlDocument"); }

            // This ensures that we're actually validating against the schema listed in the file. If the namespace in the file isn't actually included in the
            // XmlSchemaSet, it technically passes validation, which is not what we want. We want to enforce conformity.
            if (!_dbXmlSchemaSet.Contains(dbXmlDocument.Root.GetDefaultNamespace().ToString()))
            { throw new XmlSchemaValidationException("The namespace referenced in the specified XML document is not listed for validation. Call AddDbXmlSchema() to add it."); }

            // It's okay if we
            dbXmlDocument.Validate(_dbXmlSchemaSet, validationEventHandler, addSchemaInfo);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> delegate for each script generated from the database schema file.
        /// </summary>
        /// <param name="dbSchemaName">The name of the database schema to use for updates.</param>
        /// <param name="executeDelegate">The <see cref="Action"/> to execute for each script.</param>
        public void ExecuteAllScripts(string dbSchemaName, Action<string> executeDelegate)
        {
            if (string.IsNullOrWhiteSpace((dbSchemaName)))
            { throw new ArgumentNullException("dbSchemaName"); }
            if (executeDelegate == null)
            { throw new ArgumentNullException("executeDelegate"); }
            if (_dbConfig == null)
            { throw new InvalidOperationException("No schemas have been loaded. No updates performed."); }
            if (!_dbConfig.ContainsSchema(dbSchemaName))
            { throw new ArgumentException("The database schema name '" + dbSchemaName + "' was not found in the " + typeof(IDbSchemaConfiguration).Name + "."); }

            // TODO: Provide feedback in terms of progress and error messages

            var dbSchema = _dbConfig.GetSchema(dbSchemaName);

            var builder = new StringBuilder();
            foreach (var dbObject in dbSchema.GetDbObjects())
            {
                // Clear out our buffer before writing to it again
                builder.Clear();

                // Write out our safe update scripts for each of the database objects and run them individually against the database
                using (var writer = new StringWriter(builder))
                {
                    dbObject.ScriptWriter.WriteDbScript(writer);
                    writer.Flush();
                }

                // TODO: Should probably raise an event here instead of just executing a function
                executeDelegate(builder.ToString());

                // TODO: Should probably add code in here to check a cancellation token if possible. May need an additional overload that takes the token as a parameter
            }
        }
    }
}
