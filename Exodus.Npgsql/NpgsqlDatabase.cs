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

        readonly string _connectionString;
        readonly string _serverConnectionString;

        public NpgsqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
            var builder = new NpgsqlConnectionStringBuilder(_connectionString);
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
            throw new NotImplementedException();
        }
    }
}
