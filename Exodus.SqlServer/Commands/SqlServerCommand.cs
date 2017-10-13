using System;
using System.Collections.Generic;
using System.Text;
using Exodus.Database.Communication;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Exodus.SqlServer.Commands
{
    abstract class SqlServerCommand : Command<SqlConnection, SqlCommand, SqlParameterCollection>
    {
        public SqlServerCommand(string connectionString, bool isTransactional = true)
            : base(connectionString, isTransactional)
        {
        }

        protected override SqlConnection CreateConnection()
            => new SqlConnection(ConnectionString);

        protected override SqlCommand CreateCommand(SqlConnection connection)
            => new SqlCommand(Sql, connection);

        protected override Task OpenConnection(SqlConnection connection)
            => connection.OpenAsync();

        protected override Task ExecuteCommand(SqlCommand command)
            => command.ExecuteNonQueryAsync();

        protected override async Task ExecuteCommandInTransaction(SqlCommand command)
        {
            using (var transaction = command.Connection.BeginTransaction())
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
                transaction.Commit();
            }
        }
    }
}
