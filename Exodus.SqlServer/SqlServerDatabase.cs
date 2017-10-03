using Exodus.SqlServer.Commands;
using Exodus.SqlServer.Queries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.SqlServer
{
    public class SqlServerDatabase : IDatabase
    {
        public string Name { get; }

        readonly string _connectionString;
        readonly string _serverConnectionString;

        public SqlServerDatabase(string connectionString)
        {
            _connectionString = connectionString;

            var builder = new SqlConnectionStringBuilder(_connectionString);
            Name = builder.InitialCatalog;
            builder.InitialCatalog = "";
            _serverConnectionString = builder.ToString();
        }

        public Task CreateIfNotExists()
        {
            var create = new CreateDatabaseIfNotExists(_serverConnectionString, Name);
            return create.Execute();
        }

        public Task CreateMigrationsTableIfNotExists()
        {
            var create = new CreateMigrationsTableIfNotExists(_connectionString);
            return create.Execute();
        }

        public Task DropIfExists()
        {
            var drop = new DropDatabaseIfExists(_serverConnectionString, Name);
            return drop.Execute();
        }

        public Task<int[]> GetAppliedMigrationVersions()
        {
            var get = new GetAppliedMigrationVersions(_connectionString);
            return get.Execute();
        }

        public Task RunMigration(Migration migration)
        {
            var run = new RunMigration(_connectionString, migration);
            return run.Execute();
        }
    }
}
