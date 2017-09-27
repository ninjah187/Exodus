using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Communication
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
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(Sql, connection))
            {
                AddParameters(command.Parameters);
                await connection.OpenAsync();
                return await ExecuteQuery(command);
            }
        }

        protected abstract Task<TResult> ExecuteQuery(SqlCommand command);
    }
}
