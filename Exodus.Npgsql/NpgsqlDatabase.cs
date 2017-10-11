using Exodus.Database;
using System;
using System.Collections.Generic;
using System.Text;
using Exodus.Core;
using System.Threading.Tasks;
using Npgsql;
using Exodus.Npgsql.Queries;
using Exodus.Npgsql.Commands;

namespace Exodus.Npgsql
{
    public class NpgsqlDatabase : IDatabase
    {
        public string Name { get; }

        readonly string _databaseConnectionString;
        readonly string _serverConnectionString;

        public NpgsqlDatabase(string connectionString)
        {
            _databaseConnectionString = connectionString;
            var builder = new NpgsqlConnectionStringBuilder(_databaseConnectionString);
            Name = builder.Database;
            builder.Database = "";
            _serverConnectionString = builder.ToString();
        }

        public Task CreateIfNotExists()
        {
            var create = new CreateDatabaseIfNotExists(_serverConnectionString, Name);
            return create.Execute();
        }

        public Task CreateMigrationsTableIfNotExists()
        {
            var create = new CreateMigrationsTableIfNotExists(_databaseConnectionString);
            return create.Execute();
        }

        public Task DropIfExists()
        {
            var drop = new DropDatabaseIfExists(_serverConnectionString, Name);
            return drop.Execute();
        }

        public Task<int[]> GetAppliedMigrationVersions()
        {
            var get = new GetAppliedMigrationVersions(_databaseConnectionString);
            return get.Execute();
        }

        public Task RunMigration(Migration migration)
        {
            var run = new RunMigration(_databaseConnectionString, migration);
            return run.Execute();
        }
    }
}
