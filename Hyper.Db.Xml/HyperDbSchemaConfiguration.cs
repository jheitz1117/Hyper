using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using Hyper.Db.Xml.ScriptSources;
using Hyper.Db.Xml.ScriptWriters;

namespace Hyper.Db.Xml
{
    internal class HyperDbSchemaConfiguration : IDbSchemaConfiguration
    {
        private readonly Type _defaultDbTableScriptWriterType = typeof(EmptyScriptWriter);
        private readonly Type _defaultDbPrimaryKeyScriptWriterType = typeof(EmptyScriptWriter);
        private readonly Type _defaultDbForeignKeyScriptWriterType = typeof(EmptyScriptWriter);
        private readonly XNamespace _hyperDbXmlNamespace;
        private readonly ConcurrentDictionary<string, IDbSchema> _dbSchemas = new ConcurrentDictionary<string, IDbSchema>();
        private readonly ConcurrentDictionary<string, Type> _scriptWriterTypes = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Creates a new instance of <see cref="HyperDbSchemaConfiguration"/> with the specified <see cref="XNamespace"/> object.
        /// </summary>
        /// <param name="hyperDbXmlNamespace">The <see cref="XNamespace"/> object representing the default XML Schema for validation.</param>
        public HyperDbSchemaConfiguration(XNamespace hyperDbXmlNamespace)
        {
            _hyperDbXmlNamespace = hyperDbXmlNamespace;
        }

        /// <summary>
        /// Deserializes the specified <see cref="XDocument"/> object into this instance.
        /// </summary>
        /// <param name="dbXmlDocument"><see cref="XDocument"/> object containing the XML to deserialize.</param>
        public void Deserialize(XDocument dbXmlDocument)
        {
            if (dbXmlDocument == null)
                throw new ArgumentNullException(nameof(dbXmlDocument));
            if (dbXmlDocument.Root == null)
                throw new XmlSchemaValidationException("No root node exists.");

            _dbSchemas.Clear();
            _scriptWriterTypes.Clear();

            var scriptWriterParent = dbXmlDocument.Root.Element(_hyperDbXmlNamespace + "scriptWriters");
            if (scriptWriterParent != null)
                DeserializeScriptWriters(scriptWriterParent);

            var dbSchemaParent = dbXmlDocument.Root.Element(_hyperDbXmlNamespace + "databaseSchemas");
            if (dbSchemaParent != null)
                DeserializeDatabaseSchemas(dbSchemaParent);
        }

        /// <summary>
        /// Indicates whether this <see cref="IDbSchemaConfiguration"/> contains an instance of <see cref="IDbSchema"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDbSchema"/> object to find.</param>
        /// <returns></returns>
        public bool ContainsSchema(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            { throw new ArgumentNullException("name"); }

            return _dbSchemas.ContainsKey(name);
        }

        /// <summary>
        /// Returns the <see cref="IDbSchema"/> object with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDbSchema"/> object to find.</param>
        /// <returns></returns>
        public IDbSchema GetSchema(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            { throw new ArgumentNullException("name"); }

            IDbSchema schema;

            if (!_dbSchemas.TryGetValue(name, out schema))
            { throw new KeyNotFoundException("No " + typeof(IDbSchema).Name +  " with the name '" + name + "' was found in the configuration."); }

            return schema;
        }

        #region Private Methods

        /// <summary>
        /// Gets the value of the specified attribute from the specified <see cref="XElement"/> object. If the attribute doesn't exist, the specified default value is returned instead.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the specified attribute.</param>
        /// <param name="attributeName">The name of the attribute whose value to retrieve.</param>
        /// <param name="defaultValue">The value to return if the specified attribute does not exist.</param>
        /// <returns></returns>
        private static string GetOptionalAttribute(XElement parent, string attributeName, string defaultValue)
        {
            var value = defaultValue;
            if (parent.Attribute(attributeName) != null)
                value = parent.Attribute(attributeName).Value;

            return value;
        }

        /// <summary>
        /// Gets the value of the specified attribute from the specified <see cref="XElement"/> object as an Int32. If the attribute doesn't exist, the specified default value is returned instead.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the specified attribute.</param>
        /// <param name="attributeName">The name of the attribute whose value to retrieve.</param>
        /// <param name="defaultValue">The value to return if the specified attribute does not exist.</param>
        /// <returns></returns>
        private static int? GetOptionalAttributeInt32(XElement parent, string attributeName, int? defaultValue)
        {
            int value;
            var valueString = GetOptionalAttribute(parent, attributeName, null);
            return int.TryParse(valueString, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value of the specified attribute from the specified <see cref="XElement"/> object as a boolean.
        /// If the attribute doesn't exist, the specified default value is returned instead. The values "true"
        /// and "false" as well as the values "0" and "1" are considered valid values.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the specified attribute.</param>
        /// <param name="attributeName">The name of the attribute whose value to retrieve.</param>
        /// <param name="defaultValue">The value to return if the specified attribute does not exist.</param>
        /// <returns></returns>
        private static bool? GetOptionalAttributeBoolean(XElement parent, string attributeName, bool? defaultValue)
        {
            bool value;
            var valueString = GetOptionalAttribute(parent, attributeName, null);

            // Fix 0's and 1's, which are valid XML boolean values
            if (valueString == "0") valueString = bool.FalseString;
            if (valueString == "1") valueString = bool.TrueString;

            return bool.TryParse(valueString, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value of the specified attribute from the specified <see cref="XElement"/> object. If the attribute does not exist, an exception is thrown.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the specified attribute.</param>
        /// <param name="attributeName">The name of the attribute whose value to retrieve.</param>
        /// <returns></returns>
        private static string GetRequiredAttribute(XElement parent, string attributeName)
        {
            if (parent.Attribute(attributeName) == null)
            { throw new XmlSchemaValidationException("Attribute '" + attributeName + "' is required on element '" + parent.Name + "'."); }

            return parent.Attribute(attributeName).Value;
        }

        /// <summary>
        /// Creates an instance of the specified type and casts it to the type specified as the template parameter. If <typeparamref name="T"/> is an interface
        /// not implemented by <paramref name="inputType"/>, an exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type to which to cast the created object.</typeparam>
        /// <param name="inputType">The type of object to create.</param>
        /// <returns></returns>
        private static T CreateInstanceAs<T>(Type inputType)
        {
            var targetType = typeof(T);
            if (targetType.IsInterface && !inputType.GetInterfaces().Contains(targetType))
            { throw new HyperDbSchemaConfigurationException("Type '" + inputType.Name + "' must implement the '" + targetType.Name + "' interface."); }

            return (T)Activator.CreateInstance(inputType);
        }

        /// <summary>
        /// Deserializes any script writers defined in the specified <see cref="XContainer"/>.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> object containing the XML definitions for the script writers.</param>
        private void DeserializeScriptWriters(XContainer parent)
        {
            foreach (var element in parent.Elements(_hyperDbXmlNamespace + "add"))
            {
                _scriptWriterTypes.TryAdd(
                    GetRequiredAttribute(element, "name"),
                    Type.GetType(GetRequiredAttribute(element, "type"), true) // Deliberately throw an exception if we can't load the type
                );
            }
        }

        /// <summary>
        /// Deserializes any database schemas defined in the specified <see cref="XContainer"/>.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> containing the XML definitions for the database schemas.</param>
        private void DeserializeDatabaseSchemas(XContainer parent)
        {
            foreach (var schemaElement in parent.Elements(_hyperDbXmlNamespace + "databaseSchema"))
            {
                var dbSchema = new HyperDbSchema
                {
                    Name = GetRequiredAttribute(schemaElement, "name")
                };

                var versionString = GetOptionalAttribute(schemaElement, "version", null);
                Version versionObject;
                if (Version.TryParse(versionString, out versionObject))
                    dbSchema.Version = versionObject;

                var tableCollectionElement = schemaElement.Element(_hyperDbXmlNamespace + "tables");
                var storedProcedureCollectionElement = schemaElement.Element(_hyperDbXmlNamespace + "storedProcedures");
                var customDbObjectParent = schemaElement.Element(_hyperDbXmlNamespace + "customDbObjects");
                var scriptCollectionElement = schemaElement.Element(_hyperDbXmlNamespace + "scripts");

                if (tableCollectionElement != null)
                    DeserializeTables(tableCollectionElement, dbSchema);
            
                if (storedProcedureCollectionElement != null)
                    DeserializeStoredProcedures(storedProcedureCollectionElement, dbSchema);
                
                if (customDbObjectParent != null)
                    DeserializeCustomDbObjects(customDbObjectParent, dbSchema);
                
                if (scriptCollectionElement != null)
                    DeserializeScripts(scriptCollectionElement, dbSchema);

                _dbSchemas.TryAdd(dbSchema.Name, dbSchema);
            }
        }

        /// <summary>
        /// Deserializes any tables defined in the specified <see cref="XElement"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML definitions for the tables.</param>
        /// <param name="dbSchema">The <see cref="HyperDbSchema"/> object to receive the deserialized tables.</param>
        private void DeserializeTables(XElement parent, HyperDbSchema dbSchema)
        {
            var defaultScriptWriterName = GetOptionalAttribute(parent, "scriptWriter", null);

            foreach (var tableElement in parent.Elements(_hyperDbXmlNamespace + "table"))
            {
                var table = new HyperDbTable
                {
                    Name = GetRequiredAttribute(tableElement, "name")
                };

                var versionString = GetOptionalAttribute(tableElement, "version", null);
                Version versionObject;
                if (Version.TryParse(versionString, out versionObject))
                    table.Version = versionObject;

                var scriptWriterType = _defaultDbTableScriptWriterType;
                var scriptWriterName = GetOptionalAttribute(tableElement, "scriptWriter", defaultScriptWriterName);
                if (!string.IsNullOrWhiteSpace(scriptWriterName))
                {
                    if (_scriptWriterTypes.ContainsKey(scriptWriterName))
                        scriptWriterType = _scriptWriterTypes[scriptWriterName];
                    else
                        throw new HyperDbSchemaConfigurationException("Invalid " + typeof(IDbTableScriptWriter).Name + " was specified for table " + table.Name + ".");
                }

                var columnCollectionElement = tableElement.Element(_hyperDbXmlNamespace + "columns");
                if (columnCollectionElement != null)
                    DeserializeColumns(columnCollectionElement, table);

                var constraintCollectionElement = tableElement.Element(_hyperDbXmlNamespace + "constraints");
                if (constraintCollectionElement != null)
                    DeserializeConstraints(constraintCollectionElement, table);

                // Finally fill in our custom writer objects
                var scriptWriterObject = CreateInstanceAs<IDbTableScriptWriter>(scriptWriterType);
                scriptWriterObject.Source = table;
                table.ScriptWriter = scriptWriterObject;

                dbSchema.Tables.Add(table);
            }
        }

        /// <summary>
        /// Deserializes any tables defined in the specified <see cref="XContainer"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> object containing the XML definitions for the columns.</param>
        /// <param name="table">The <see cref="HyperDbTable"/> object to receive the deserialized columns.</param>
        private void DeserializeColumns(XContainer parent, HyperDbTable table)
        {
            foreach (var columnElement in parent.Elements(_hyperDbXmlNamespace + "column"))
            {
                table.Columns.Add(new HyperDbColumn()
                {
                    Name = GetRequiredAttribute(columnElement, "name"),
                    Type = GetRequiredAttribute(columnElement, "type"),
                    MaxLength = GetOptionalAttributeInt32(columnElement, "maxLength", null),
                    Decimals = GetOptionalAttributeInt32(columnElement, "decimals", null),
                    IsNullable = GetOptionalAttributeBoolean(columnElement, "nullable", null),
                    DefaultValue = GetOptionalAttribute(columnElement, "defaultValue", null)
                });
            }
        }

        /// <summary>
        /// Deserializes any constraints defined in the specified <see cref="XContainer"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> object containing the XML definitions for the constraints.</param>
        /// <param name="table">The <see cref="HyperDbTable"/> object to receive the deserialized constraints.</param>
        private void DeserializeConstraints(XContainer parent, HyperDbTable table)
        {
            var primaryKeyElement = parent.Element(_hyperDbXmlNamespace + "primaryKey");
            if (primaryKeyElement != null)
                DeserializePrimaryKey(primaryKeyElement, table);

            var foreignKeyCollectionElement = parent.Element(_hyperDbXmlNamespace + "foreignKeys");
            if (foreignKeyCollectionElement != null)
                DeserializeForeignKeys(foreignKeyCollectionElement, table);
        }

        /// <summary>
        /// Deserializes any primary key columns defined in the specified <see cref="XElement"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML definitions for the primary key columns.</param>
        /// <param name="table">The <see cref="HyperDbTable"/> object to receive the deserialized primary key columns.</param>
        private void DeserializePrimaryKey(XElement parent, HyperDbTable table)
        {
            var scriptWriterType = _defaultDbPrimaryKeyScriptWriterType;
            var scriptWriterName = GetOptionalAttribute(parent, "scriptWriter", null);
            if (!string.IsNullOrWhiteSpace(scriptWriterName))
            {
                if (_scriptWriterTypes.ContainsKey(scriptWriterName))
                    scriptWriterType = _scriptWriterTypes[scriptWriterName];
                else
                    throw new HyperDbSchemaConfigurationException("Invalid " + typeof(IDbPrimaryKeyScriptWriter).Name +" was specified for table " + table.Name + ".");
            }

            table.PrimaryKey = new HyperDbPrimaryKey
            {
                PrimaryKeyName = GetOptionalAttribute(parent, "name", null),
                TableSource = table
            };

            // Per our XML schema, if we have a primaryKey element, we will always have at least one keyColumn element
            foreach (var keyColumnElement in parent.Elements(_hyperDbXmlNamespace + "keyColumn"))
            {
                table.PrimaryKey.KeyColumns.Add(
                    GetRequiredAttribute(keyColumnElement, "name")
                );
            }

            // Fill in our script writer
            var scriptWriterObject = CreateInstanceAs<IDbPrimaryKeyScriptWriter>(scriptWriterType);
            scriptWriterObject.Source = table.PrimaryKey;
            table.PrimaryKey.ScriptWriter = scriptWriterObject;
        }

        /// <summary>
        /// Deserializes any foreign keys defined in the specified <see cref="XElement"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML definitions for the foreign keys.</param>
        /// <param name="table">The <see cref="HyperDbTable"/> object to receive the deserialized foreign keys.</param>
        private void DeserializeForeignKeys(XElement parent, HyperDbTable table)
        {
            var defaultScriptWriterName = GetOptionalAttribute(parent, "scriptWriter", null);

            foreach (var foreignKeyElement in parent.Elements(_hyperDbXmlNamespace + "foreignKey"))
            {
                var foreignKey = new HyperDbForeignKey
                {
                    // Name attribute is required because multiple foreign keys with the same definition are allowed to exist in some (all?) databases.
                    // The only way to uniquely identify one is by name.
                    ForeignKeyName = GetRequiredAttribute(foreignKeyElement, "name"), 
                    ReferencedTableName = GetRequiredAttribute(foreignKeyElement, "referencedTable"),
                    TableSource = table
                };

                var scriptWriterType = _defaultDbForeignKeyScriptWriterType;
                var scriptWriterName = GetOptionalAttribute(foreignKeyElement, "scriptWriter", defaultScriptWriterName);
                if (!string.IsNullOrWhiteSpace(scriptWriterName))
                {
                    if (_scriptWriterTypes.ContainsKey(scriptWriterName))
                        scriptWriterType = _scriptWriterTypes[scriptWriterName];
                    else
                        throw new HyperDbSchemaConfigurationException("Invalid " + typeof(IDbForeignKeyScriptWriter).Name + " was specified for table " + table.Name + ".");
                }

                // Per our XML schema, if we have a foreignKey element, we will always have at least one keyColumn element
                foreach (var keyColumnElement in foreignKeyElement.Elements(_hyperDbXmlNamespace + "keyColumn"))
                {
                    foreignKey.KeyColumns.Add(
                        new DbForeignKeyMapping
                        {
                            ForeignKeyColumnName = GetRequiredAttribute(keyColumnElement, "name"),
                            ReferencedColumnName = GetRequiredAttribute(keyColumnElement, "referencedColumn")
                        }
                    );
                }

                // Fill in our script writer
                var scriptWriterObject = CreateInstanceAs<IDbForeignKeyScriptWriter>(scriptWriterType);
                scriptWriterObject.Source = foreignKey;
                foreignKey.ScriptWriter = scriptWriterObject;

                table.ForeignKeys.Add(foreignKey);
            }
        }

        /// <summary>
        /// Deserializes any custom database objects defined in the specified <see cref="XElement"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML definitions for the stored procedures.</param>
        /// <param name="dbSchema">The <see cref="HyperDbSchema"/> object to receive the deserialized tables.</param>
        private void DeserializeCustomDbObjects(XElement parent, HyperDbSchema dbSchema)
        {
            var customDbConfigType = Type.GetType(GetRequiredAttribute(parent, "type"), true); // Deliberately throw an exception if we can't load the type
            var customDbConfigObject = CreateInstanceAs<IDbCustomConfiguration>(customDbConfigType);

            customDbConfigObject.Deserialize(parent, _scriptWriterTypes);

            dbSchema.DbCustomConfig = customDbConfigObject;
        }

        /// <summary>
        /// Deserializes any stored procedures defined in the specified <see cref="XContainer"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> object containing the XML definitions for the stored procedures.</param>
        /// <param name="dbSchema">The <see cref="HyperDbSchema"/> object to receive the deserialized tables.</param>
        private void DeserializeStoredProcedures(XContainer parent, HyperDbSchema dbSchema)
        {
            // TODO: Phase 2 item
        }

        /// <summary>
        /// Deserializes any scripts defined in the specified <see cref="XContainer"/> object into the specified <see cref="HyperDbSchema"/> object.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> object containing the XML definitions for the scripts.</param>
        /// <param name="dbSchema">The <see cref="HyperDbSchema"/> object to receive the deserialized tables.</param>
        private void DeserializeScripts(XContainer parent, HyperDbSchema dbSchema)
        {
            // TODO: Phase 2 item
        }

        #endregion Private Methods
    }
}
