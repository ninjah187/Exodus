using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Threading.Tasks;

namespace Exodus.Npgsql.Commands
{
    class RunMigration : PostgresCommand
    {
        readonly Migration _migration;

        public RunMigration(string databaseConnectionString, Migration migration)
            : base(databaseConnectionString)
        {
            _migration = migration;
            Sql = migration.Script;
        }

        protected async override Task ExecuteCommandInTransaction(NpgsqlCommand runMigration)
        {
            var connection = runMigration.Connection;
            var updateMigrationsInfo = CreateUpdateMigrationsInfoCommand(connection);
            using (var transaction = connection.BeginTransaction())
            {
                runMigration.Transaction = transaction;
                updateMigrationsInfo.Transaction = transaction;
                await runMigration.ExecuteNonQueryAsync();
                await updateMigrationsInfo.ExecuteNonQueryAsync();
                transaction.Commit();
            }
        }

        NpgsqlCommand CreateUpdateMigrationsInfoCommand(NpgsqlConnection connection)
        {
            var sql = "INSERT INTO Migrations VALUES (@version, @appliedOn, @name)";
            var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add(new NpgsqlParameter("version", _migration.Version));
            command.Parameters.Add(new NpgsqlParameter("appliedOn", DateTime.Now));
            command.Parameters.Add(new NpgsqlParameter("name", _migration.Name));
            return command;
        }
    }
}
