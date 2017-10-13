using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Exodus.Database.Communication;
using Npgsql;

namespace Exodus.Npgsql.Commands
{
    abstract class PostgresCommand : Command<NpgsqlConnection, NpgsqlCommand, NpgsqlParameterCollection>
    {
        public PostgresCommand(string connectionString, bool isTransactional = true)
            : base(connectionString, isTransactional)
        {
        }

        protected override NpgsqlConnection CreateConnection()
            => new NpgsqlConnection(ConnectionString);

        protected override NpgsqlCommand CreateCommand(NpgsqlConnection connection)
            => new NpgsqlCommand(Sql, connection);

        protected override Task OpenConnection(NpgsqlConnection connection)
            => connection.OpenAsync();

        protected override Task ExecuteCommand(NpgsqlCommand command)
            => command.ExecuteNonQueryAsync();

        protected override async Task ExecuteCommandInTransaction(NpgsqlCommand command)
        {
            using (var transaction = command.Connection.BeginTransaction())
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
        }
    }
}
