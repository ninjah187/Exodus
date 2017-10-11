using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Npgsql
{
    abstract class Command : Message
    {
        readonly bool _isTransactional;

        public Command(string connectionString, bool isTransactional = true)
            : base(connectionString)
        {
            _isTransactional = isTransactional;
        }

        public async Task Execute()
        {
            Validate();
            using (var connection = new NpgsqlConnection(ConnectionString))
            using (var command = new NpgsqlCommand(Sql, connection))
            {
                AddParameters(command.Parameters);
                await connection.OpenAsync();
                if (_isTransactional)
                {
                    await ExecuteInTransaction(connection, command);
                }
                else
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        async Task ExecuteInTransaction(NpgsqlConnection connection, NpgsqlCommand command)
        {
            using (var transaction = connection.BeginTransaction())
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
        }
    }
}
