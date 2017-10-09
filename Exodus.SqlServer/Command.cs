using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.SqlServer
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
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(Sql, connection))
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

        async Task ExecuteInTransaction(SqlConnection connection, SqlCommand command)
        {
            using (var transaction = connection.BeginTransaction())
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
                transaction.Commit();
            }
        }
    }
}
