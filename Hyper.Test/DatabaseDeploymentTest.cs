using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Hyper.Db.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyper.Test
{
    [TestClass]
    public class DatabaseDeploymentTest
    {
        // Job of the consuming service:
        //    - To extract the XML from a file, or database or whatever and create an XDocument object
        //    - Creates a HyperDbXmlScriptProvider instance to handle the updates
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
                var dbManager = new HyperDbXmlScriptProvider();
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
                var dbManager = new HyperDbXmlScriptProvider();
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
                var dbManager = new HyperDbXmlScriptProvider();
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
            var doc = XDocument.Parse("<databaseSchemaConfiguration xmlns=\"http://www.hyperdb.com/HyperDbXmlSchema\" />");
            var dbManager = new HyperDbXmlScriptProvider();
            dbManager.ValidateDbXmlDocument(doc, null, false);
        }

        [TestMethod]
        public void ValidateXmlDeclarationNoRootTest()
        {
            Exception testEx = null;

            try
            {
                var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                var dbManager = new HyperDbXmlScriptProvider();
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
                var dbManager = new HyperDbXmlScriptProvider();
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
            var dbManager = new HyperDbXmlScriptProvider();
            dbManager.ValidateDbXmlDocument(doc, null, false);
        }

        [TestMethod]
        public void ValidateCustomDbSchemaTest()
        {
            var doc = XDocument.Load("TestCustomDbSchema.xml");
            var dbManager = new HyperDbXmlScriptProvider();
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
                var dbManager = new HyperDbXmlScriptProvider();
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
                var dbManager = new HyperDbXmlScriptProvider();
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

        [TestMethod]
        public void LoadHyperDbSchemaTest()
        {
            var doc = XDocument.Load("TestHyperDbSchema.xml");
            var scriptProvider = new HyperDbXmlScriptProvider();
            scriptProvider.LoadDbXmlDocument(doc, true);

            var builder = new StringBuilder();
            Debug.Listeners.Clear();
            Debug.Listeners.Add(
                new TextWriterTraceListener(
                    new StringWriter(builder)
                )
            );
            
            scriptProvider.ExecuteAllScripts("TestSchema", WriteDebugLine);
        }

        private static void WriteDebugLine(string line)
        {
            Debug.WriteLine(line);
        }
    }
}
