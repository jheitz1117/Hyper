using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using Hyper.Core.Cryptography;
using Hyper.Cryptography;
using Hyper.Db.ScriptWriters.Sql;
using Hyper.Db.Xml;
using Hyper.IO;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using NodeModuleTest.Contracts.DbUpdate;

namespace NodeModuleTest.CommandModules.DbUpdate
{
    public class DbUpdateCommand : ICommandModule, ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
    {
        private static string DatabaseSchemaXmlFilePath = "DbUpdate.xml";
        private static readonly string HyperGatewayConnectionString = ConfigurationManager.ConnectionStrings["HyperGateway"].ConnectionString;
        private static readonly SymmetricEncryptionService encryption = new SymmetricEncryptionService(
            new SymmetricEncryptionConfiguration
            {
                AlgorithmType = SymmetricAlgorithmType.Aes,
                CipherMode = CipherMode.CBC,
                PaddingMode = PaddingMode.PKCS7,
                IvTransform = StringTransform.GetHexTransform(),
                KeyTransform = StringTransform.GetHexTransform(),
                CipherTextTransform = StringTransform.GetBase64Transform(),
                PlainTextTransform = StringTransform.FromEncoding(Encoding.Default)
            }
        );

        private readonly object _serializer;

        public DbUpdateCommand()
        {
            _serializer = new DataContractCommandSerializer<DbUpdateRequest, DbUpdateResponse>();
        }

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as DbUpdateRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(DbUpdateRequest), context.Request.GetType());

            var response = new DbUpdateResponse();

            try
            {
                var dbConfigProvider = new XmlDbSchemaProvider();

                // Now we can load and validate the XML document
                dbConfigProvider.LoadDbXmlDocument(
                    XDocument.Load(DatabaseSchemaXmlFilePath),
                    true
                );

                // Finally, we can retrieve our deserialized model
                var dbConfig = dbConfigProvider.GetDbSchemas();

                // Use our development script writer for now
                var scriptWriter = new DevelopmentSqlScriptWriter();

                // Now grab our list of targets
                IEnumerable<DbUpdateTarget> targets;
                using (var connection = new SqlConnection(HyperGatewayConnectionString))
                {
                    connection.Open();

                    var dataProvider = new DbUpdateDataProvider(connection);

                    context.Activity.Track("Retrieving database update targets...");
                    targets = dataProvider.GetDbUpdateTargets(Environment.MachineName, context.ExecutingNodeName);

                    // This filters the targets to only the databases requested by the user
                    if (request.TargetDatabases?.Any() ?? false)
                        targets = targets.Where(t => request.TargetDatabases.Contains(t.DatabaseName));

                    context.Activity.Track("Successfully retrieved database update targets.");
                    connection.Close();
                }

                foreach (var target in targets)
                {
                    try
                    {
                        if (!dbConfig.ContainsSchema(target.DatabaseSchemaName))
                            throw new KeyNotFoundException($"The database schema '{target.DatabaseSchemaName}' could not be found in the file '{DatabaseSchemaXmlFilePath}'.");

                        var dbSchema = dbConfig.GetSchema(target.DatabaseSchemaName);

                        var dataSource = target.ServerName;
                        if (!string.IsNullOrWhiteSpace(target.InstanceName))
                            dataSource += $"\\{target.InstanceName}";

                        var builder = new SqlConnectionStringBuilder
                        {
                            DataSource = dataSource,
                            InitialCatalog = target.DatabaseName,
                            UserID = target.UserName,
                            Password = Decrypt(target.Password)
                        };
                        
                        Func<Action<TextWriter>, string> getSql = writerDelegate =>
                        {
                            var queryBuilder = new StringBuilder();
                            using (var writer = new StringWriter(queryBuilder))
                            {
                                writerDelegate(writer);
                                writer.Flush();
                            }

                            return queryBuilder.ToString();
                        };

                        using (var transaction = new TransactionScope())
                        {
                            using (var targetConnection = new SqlConnection(builder.ConnectionString))
                            {
                                targetConnection.Open();

                                // Tables first
                                foreach (var table in dbSchema.Tables)
                                {
                                    using (var command = targetConnection.CreateCommand())
                                    {
                                        command.CommandText = getSql(w => scriptWriter.WriteDbScript(w, table));
                                        command.ExecuteNonQuery();
                                    }
                                }

                                // Now primary keys
                                foreach (var primaryKey in dbSchema.PrimaryKeys)
                                {
                                    using (var command = targetConnection.CreateCommand())
                                    {
                                        command.CommandText = getSql(w => scriptWriter.WriteDbScript(w, primaryKey));
                                        command.ExecuteNonQuery();
                                    }
                                }

                                // Now foreign keys
                                foreach (var foreignKey in dbSchema.ForeignKeys)
                                {
                                    using (var command = targetConnection.CreateCommand())
                                    {
                                        command.CommandText = getSql(w => scriptWriter.WriteDbScript(w, foreignKey));
                                        command.ExecuteNonQuery();
                                    }
                                }

                                targetConnection.Close();
                            }

                            transaction.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Activity.TrackException(ex);
                        response.ProcessStatusFlags |= MessageProcessStatusFlags.HadNonFatalErrors;
                    }
                }
            }
            catch
            {
                response.ProcessStatusFlags |= MessageProcessStatusFlags.Failure;
                throw;
            }
            
            // TODO: Figure out what response data should be returned.
            return response;
        }

        ICommandRequestSerializer ICommandRequestSerializerFactory.Create()
        {
            return _serializer as ICommandRequestSerializer;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return _serializer as ICommandResponseSerializer;
        }

        private static string Decrypt(string input)
        {
            return encryption.DecryptString(
                input,
                "19fbfe05d30e8134d1f383fe9134369cce04034817d2da6d0c757beb394862e0",
                "edee4b1cdd9314ec6d8c877f3b97352b"
            );
        }
    }
}
