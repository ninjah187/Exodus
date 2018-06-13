using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.SqlServer.Commands
{
    class RunMigration : SqlServerCommand
    {
        readonly Migration _migration;

        public RunMigration(string connectionString, Migration migration)
            : base(connectionString)
        {
            _migration = migration;
            Sql = migration.Script;
        }

        protected async override Task ExecuteCommandInTransaction(SqlCommand runMigration)
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

        SqlCommand CreateUpdateMigrationsInfoCommand(SqlConnection connection)
        {
            var sql = "INSERT INTO Migrations VALUES (@version, @appliedOn, @name)";
            var command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("version", _migration.Version));
            command.Parameters.Add(new SqlParameter("appliedOn", DateTime.Now));
            command.Parameters.Add(new SqlParameter("name", _migration.Name));
            return command;
        }
    }
}
