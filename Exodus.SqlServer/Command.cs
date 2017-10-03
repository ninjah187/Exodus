using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.SqlServer
{
    abstract class Command : Message
    {
        public Command(string connectionString)
            : base(connectionString)
        {
        }

        public async Task Execute()
        {
            Validate();
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(Sql, connection))
            {
                AddParameters(command.Parameters);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
