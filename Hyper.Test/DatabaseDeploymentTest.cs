using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Hyper.Db;
using Hyper.Db.ScriptWriters.Sql;
using Hyper.Db.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyper.Test
{
    [TestClass]
    public class DatabaseDeploymentTest
    {
        // Job of the consuming service:
        //    - To extract the XML from a file, or database or whatever and create an XDocument object
        //    - Creates a XmlDbSchemaProvider instance to handle the updates
        //    - Calls the provider's LoadDbXmlDocument() method passing the XDocument it just created
        //    - Loads up a mapping of connection strings and schema names
        //    - Calls the provider's ExecuteAllScripts() method for each connection string using the appropriate schema name

        [TestMethod]
        public void ValidateEmptyDocumentTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlException);
        }

        [TestMethod]
        public void ValidateWhitespaceDocumentTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("     \t\t \r\n\r\n    ");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlException);
        }

        [TestMethod]
        public void ValidateMultipleRootNodeTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("<root1 /><root2 />");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlException);
        }

        [TestMethod]
        public void ValidateNoXmlDeclarationTest()
        {
            var doc = XDocument.Parse("<databaseSchemaConfiguration xmlns=\"http://www.hypersoa.net/HyperDbXmlSchema\" />");
            var dbManager = new XmlDbSchemaProvider();
            dbManager.ValidateDbXmlDocument(doc, null, false);
        }

        [TestMethod]
        public void ValidateXmlDeclarationNoRootTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlException);
        }

        [TestMethod]
        public void ValidateNoNamespaceTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?><databaseSchemas />");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.AddDbXmlSchema(null, "TestCustomDbXmlSchema.xsd");
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlSchemaValidationException);
        }

        [TestMethod]
        public void ValidateHyperDbSchemaTest()
        {
            var doc = XDocument.Load("TestHyperDbSchema.xml");
            var dbManager = new XmlDbSchemaProvider();
            dbManager.ValidateDbXmlDocument(doc, null, false);
        }

        [TestMethod]
        public void ValidateCustomDbSchemaTest()
        {
            var doc = XDocument.Load("TestCustomDbSchema.xml");
            var dbManager = new XmlDbSchemaProvider();
            dbManager.ClearDbXmlSchemas();
            dbManager.AddDbXmlSchema(null, "TestCustomDbXmlSchema.xsd");
            dbManager.ValidateDbXmlDocument(doc, null, false);
        }

        [TestMethod]
        public void ValidateCustomAgainstHyperDbSchemaTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Load("TestCustomDBSchema.xml");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlSchemaValidationException);
        }

        [TestMethod]
        public void ValidateHyperAgainstCustomDbSchemaTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Load("TestHyperDbSchema.xml");
                var dbManager = new XmlDbSchemaProvider();
                dbManager.ClearDbXmlSchemas();
                dbManager.AddDbXmlSchema(null, "TestCustomDbXmlSchema.xsd");
                dbManager.ValidateDbXmlDocument(doc, null, false);
            }
            catch (Exception ex)
            {
                testEx = ex;
            }

            // If the expected exception was thrown, the test was successful
            Assert.IsNotNull(testEx);
            Assert.IsTrue(testEx is XmlSchemaValidationException);
        }

        //[TestMethod]
        //public void LoadHyperDbSchemaTest()
        //{
        //    var doc = XDocument.Load("TestHyperDbSchema.xml");
        //    var scriptProvider = new XmlDbSchemaProvider();
        //    scriptProvider.LoadDbXmlDocument(doc, true);

        //    // TODO: Note that I think an "execute all scripts" function may not be correct here. Instead, we should generate a script per object and execute each one individually to avoid conflicts in variable names. Furthermore, we should consider using a transaction for all DB updates so that if there are problems, we don't end up with a partial update.
        //    scriptProvider.ExecuteAllScripts("TestSchema", WriteDebugLine);
        //}

        [TestMethod]
        public void WriteAllScriptsTest()
        {
            var dbConfigProvider = new XmlDbSchemaProvider();

            /* In this intermediary area, we might add additional XML schemas to support custom DB config types I have defined myself.*/
            /*
             * dbConfigProvider.AddXmlSchema(...);
             */

            // Now we can load and validate the XML document
            dbConfigProvider.LoadDbXmlDocument(
                XDocument.Load("TestHyperDbSchema.xml"),
                true
            );

            // Finally, we can retrieve our deserialized model
            var dbConfig = dbConfigProvider.GetDbSchemas();

            /*
             * At this point, I have utilized my provider to obtain an instance of IDbSchemaCollection. However, the config may contain
             * multiple IDbSchema instances, so I'll need to pull these out according to the business requirements and execute
             * them against the appropriate connection string.
             * 
             * As an example, maybe the software solution consists of two products: CrmPlatform and DataAnalytics. Each product is sold
             * separately, so each has its own database. Now, since this is a multitenant example, there must be a central DB that controls
             * which company goes with which database, so we actually have three DB schemas, with the third being the Tenant database.
             * 
             * This mapping would be stored somewhere and retrieved by the update tool or service when it came time to update the database.
             */
            var targetMappings = new Dictionary<string, List<string>>
            {
                {
                    "Tenants",
                    new List<string>
                    {
                        // In reality, this would be a full connection string, but for brevity in this example, we'll just list the DB name.
                        "TenantDb1",

                        /*
                         * If this was a really big project, we could potentially have multiple DBs distributed across multiple servers
                         * to contain all the tenant info
                         */
                        //"TenantDb2",
                        //"TenantDb3"
                    }
                },
                {
                    "CrmPlatform",
                    new List<string>
                    {
                        /*
                         * Each tenant could name their DB whatever they wanted, or we could control the name. What matters is the mapping
                         * between the schema name and the name of the DB. In any case, each of the companies below subscribes to the CRM
                         * platform
                         */
                        "InfraCRM",
                        "EpicCRM",
                        "MyCompanyDoesCRM"
                    }
                },
                {
                    "DataAnalytics",
                    new List<string>
                    {
                        /*
                         * This mapping allows for any combination of DB schemas to be distributed across any number of DBs. You can have one DB per schema per customer,
                         * or all schemas in the same DB (as long as they don't conflict), or you can have some customers that have some schemas and not others.
                         */
                        "InfraCRM", // Note that if the DB schemas don't conflict, there's no reason you can't point two different schemas to the same DB to consolidate the table set.
                        "EpicCRMDataAnalytics", // On the other hand, perhaps some policy requires you to separate out the DBs according to product. No problem, just provide the connection string here.
                        "DAUnity" // This company only subscribes to the data analytics product and do not have the CRM product (they don't have an entry above)
                    }
                },
            };

            /*
             * I need one last thing before I can start running the update scripts: a script writer.
             * 
             * The purpose of the script writer is to take the abstract DB objects and turn them into useful scripts. The script writers are where the real power of this framework
             * comes into play: Not only can we have different writers for each DB type (T-SQL, MySql, etc.), but we can define writers for each *environment* we want to run against.
             * For example, the writer I use in my development environment can be a lot more "harsh" or "risky" than the one that should be used in Production. In development, I can
             * have a writer that drops columns or changes column types, or whatever because I have complete control over the environment and can adjust accordingly if something goes
             * wrong. On the other hand, virtually none of that is true in Production. Therefore, the strategy and needs for updating a production database is different than the
             * strategy and needs of updating my development environment.
             * 
             * In the end, the DB schema should look the same in every environment. It's only the script used to get us there that should be different. So in Production, we can have
             * a writer that only ever generates non-destructive SQL, and then we can have a migration script that handles anything that can't be represented as "additive". In fact,
             * we don't even have to agree on a definition of "non-destructive", since the user can create their own writers with their own definition of non-destructive.
             * 
             * As far as the data migrations, I'm thinking that they should really wait to be written until shortly before release time anyway, since the DB schema could certainly change
             * many times between releases. The best part is that someone could do a diff between the XML DB config file used in the last release, versus the XML DB config file we're about
             * release and get an absolute and perfectly accurate view of every single thing that changed in the DB schema. An intelligent plan can then be executed to perform any necessary
             * data migrations. This schema seems far superior to forcing each individual developer to write their own data migration with the code they check in. In the first place, the
             * developer is only going to have a view of the required changes that is as wide as his project. If it is part of a larger project, he may not account for everything that needs
             * to happen. In fact, it is possible to find yourself in a situation such that no data migrations can be written at all until everyone finishes their respective pieces. Then
             * what do you do? So really, waiting until shortly before your release is a good time because by then your DB schema will have stabalized and a single human will be perfectly
             * capable of doing the schema diff and determining which data migrations are necessary.
             */

            // For now, I'll just use the dev script writer
            var scriptWriter = new DevelopmentSqlScriptWriter();

            /*
             * If I've added custom objects, then I'll also have to implement my own writer that recognizes those objects. I might have to implement multiple custom writers for
             * the different environments, if it matters for my objects. In any case, I can choose to create the writer in whichever way I choose, but my first thought is to use
             * a decorator so I can just delegate the existing functionality to what's already written.
             * 
             * Again, as above, hopefully this custom writer is something I can specify in a config file somewhere.
             */
            var customWriter = new AwesomeObjectScriptWriterDecorator(scriptWriter);

            /*
             * At this point, we have:
             * 1)   Obtained an instance of IDbSchemaCollection (in particular, we used XML, but we could have gotten it from any number of sources)
             * 2)   Obtained a data structure mapping DB schemas to connection strings
             * 3)   Obtained a script writer to use in writing out the DB changes.
             */
            foreach (var targetMapping in targetMappings)
            {
                var dbSchemaName = targetMapping.Key;
                if (!dbConfig.ContainsSchema(dbSchemaName))
                    continue;

                var dbSchema = dbConfig.GetSchema(dbSchemaName);

                foreach (var connectionString in targetMapping.Value)
                {
                    //RunAgainstDatabase(connectionString, dbSchema, customWriter);
                    WriteToDebug(connectionString, dbSchema, customWriter);
                }
            }
        }

        private static void WriteDebugLine(string line)
        {
            Trace.WriteLine(line);
        }

        private static void WriteToDebug(string connectionString, IDbSchema dbSchema, IDbScriptWriter scriptWriter)
        {
            // which connection string?
            Trace.WriteLine($"Using connection string '{connectionString}'.");

            // Tables first
            foreach (var table in dbSchema.Tables)
            {
                Trace.WriteLine(GetSql(w => scriptWriter.WriteDbScript(w, table)));
                Trace.WriteLine("GO");
            }

            // Now primary keys
            foreach (var primaryKey in dbSchema.PrimaryKeys)
            {
                Trace.WriteLine(GetSql(w => scriptWriter.WriteDbScript(w, primaryKey)));
                Trace.WriteLine("GO");
            }

            // Now foreign keys
            foreach (var foreignKey in dbSchema.ForeignKeys)
            {
                Trace.WriteLine(GetSql(w => scriptWriter.WriteDbScript(w, foreignKey)));
                Trace.WriteLine("GO");
            }

            // TODO: Write stored procedures
            // TODO: Write functions

            // Now custom objects
            var customSchema = dbSchema as XmlDbSchema;
            var customWriter = scriptWriter as AwesomeObjectScriptWriterDecorator;
            if (customSchema?.XmlDbObjects != null && customWriter != null)
            {
                foreach (var xmlDbObject in customSchema.XmlDbObjects.GetDbObjects())
                {
                    // test for custom object type
                    var awesomeObject = xmlDbObject as AwesomeObject;
                    if (awesomeObject != null)
                    {
                        Trace.WriteLine(GetSql(w => customWriter.WriteDbScript(w, awesomeObject)));
                        Trace.WriteLine("GO");
                    }
                }
            }
        }

        private static void RunAgainstDatabase(string connectionString, IDbSchema dbSchema, IDbScriptWriter scriptWriter)
        {
            /*
                     * I'm pretty sure if we use a transaction, the whole database will be locked up until the transaction completes. If so, this is not
                     * desirable, since there may be a lot of traffic on the database. Instead, we'll probably want to execute the updates one at a time,
                     * at least the ones that can be executed without danger (like new columns on the end of a table with no constraints).
                     * 
                     * TL;DR: We may or may not want to use a transaction.
                     */
            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // TODO: Somehow we need to loop through all the DB objects and execute their scripts against the connection string (see comments below).
                    /*
                     * How should this be accomplished? Just brainstorming, we have:
                     * 1)   Simply loop through all the DB objects and write each one with the appropriate script writer
                     *      -   But how can we determine which writer to use for each object? Should the script provider
                     *          just check if the object provided implements a specific IDbObject interface, such as IDbTable or whatever, and
                     *          then choose the writer based on that? 
                     *          -   Suppose we do it that way though, and suppose the user has some custom objects. Well, we might be able to
                     *              determine that an object is a custom object, but what if they have multiple types of custom objects? We
                     *              can't know beforehand what all those types are, so we can't write code to check their types. Instead,
                     *              we might have to expose a way for them to provide their own script writer to use for custom objects.
                     *          -   With this technique, the script writer not only needs to take dbObjects, but also any custom script
                     *              writers as necessary
                     * 2)   I'm not even sure what other ways we could do this...
                     */

                    /******************************************************************************
                     * Method 1 - Loop through all DB objects and just write 'em all out
                     ******************************************************************************/

                    // Tables first
                    foreach (var table in dbSchema.Tables)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = GetSql(w => scriptWriter.WriteDbScript(w, table));
                            command.ExecuteNonQuery();
                        }
                    }

                    // Now primary keys
                    foreach (var primaryKey in dbSchema.PrimaryKeys)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = GetSql(w => scriptWriter.WriteDbScript(w, primaryKey));
                            command.ExecuteNonQuery();
                        }
                    }

                    // Now foreign keys
                    foreach (var foreignKey in dbSchema.ForeignKeys)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = GetSql(w => scriptWriter.WriteDbScript(w, foreignKey));
                            command.ExecuteNonQuery();
                        }
                    }
                    /******************************************************************************
                     * End Method 1
                     ******************************************************************************/

                    connection.Close();
                }

                transaction.Complete();
            }
        }

        private static string GetSql(Action<TextWriter> writerDelegate)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                writerDelegate(writer);
                writer.Flush();
            }

            return builder.ToString();
        }
    }
}
