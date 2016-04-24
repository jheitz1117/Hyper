using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace NodeModuleTest.CommandModules.DbUpdate
{
    internal class DbUpdateTarget
    {
        public string ServerName { get; set; }
        public string InstanceName { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseSchemaName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    internal interface IDbUpdateDataProvider
    {
        IEnumerable<DbUpdateTarget> GetDbUpdateTargets(string hyperNodeMachineName, string hyperNodeName);
    }

    internal class DbUpdateDataProvider : IDbUpdateDataProvider
    {
        private readonly SqlConnection _connection;

        /// <summary>
        /// Initializes an instance of <see cref="DbUpdateDataProvider"/> using the specified open <see cref="SqlConnection"/> object.
        /// </summary>
        /// <param name="connection">The open <see cref="SqlConnection"/> to use to retrieve data.</param>
        public DbUpdateDataProvider(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connection = connection;
        }

        public IEnumerable<DbUpdateTarget> GetDbUpdateTargets(string hyperNodeMachineName, string hyperNodeName)
        {
            if (string.IsNullOrWhiteSpace(hyperNodeMachineName))
                throw new ArgumentNullException(nameof(hyperNodeMachineName), "Argument cannot be null or consist only of whitespace.");
            if (string.IsNullOrWhiteSpace(hyperNodeName))
                throw new ArgumentNullException(nameof(hyperNodeName), "Argument cannot be null or consist only of whitespace.");

            var targets = new List<DbUpdateTarget>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                    select
	                    dbs.ServerName,
	                    dbs.InstanceName,
	                    db.DatabaseName,
	                    dun.DatabaseSchemaName,
	                    db.UserName,
	                    db.Password
                    from DatabaseUpdateNodes dun
	                    inner join HyperNodes hn on hn.HyperNodeId = dun.HyperNodeId
	                    inner join Databases db on db.DatabaseId = dun.DatabaseId
	                    inner join DatabaseServers dbs on dbs.DatabaseServerId = db.DatabaseServerId
                    where
		                    hn.HyperNodeMachineName = @HyperNodeMachineName
	                    and hn.HyperNodeName        = @HyperNodeName";

                command.Parameters.Add("@HyperNodeMachineName", SqlDbType.NVarChar, 50).Value = hyperNodeMachineName;
                command.Parameters.Add("@HyperNodeName", SqlDbType.NVarChar, 50).Value = hyperNodeName;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        targets.Add(
                            new DbUpdateTarget
                            {
                                ServerName = reader["ServerName"] as string,
                                InstanceName = reader["InstanceName"] as string,
                                DatabaseName = reader["DatabaseName"] as string,
                                DatabaseSchemaName = reader["DatabaseSchemaName"] as string,
                                UserName = reader["UserName"] as string,
                                Password = reader["Password"] as string
                            }
                        );
                    }
                }
            }

            return targets;
        }
    }
}
