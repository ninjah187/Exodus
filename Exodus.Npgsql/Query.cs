using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Npgsql
{
    abstract class Query<TResult> : Message
    {
        public Query(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<TResult> Execute()
        {
            Validate();
            using (var connection = new NpgsqlConnection(ConnectionString))
            using (var command = new NpgsqlCommand(Sql, connection))
            {
                AddParameters(command.Parameters);
                await connection.OpenAsync();
                return await ExecuteQuery(command);
            }
        }

        protected abstract Task<TResult> ExecuteQuery(NpgsqlCommand command);
    }
}
