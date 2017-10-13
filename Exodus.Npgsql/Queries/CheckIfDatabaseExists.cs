using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Exodus.Npgsql.Queries
{
    class CheckIfDatabaseExists : PostgresQuery<bool>
    {
        public CheckIfDatabaseExists(string serverConnectionString, string databaseName)
            : base(serverConnectionString)
        {
            Sql = $@"
                SELECT 1 FROM pg_database WHERE datname = '{databaseName}';
            ";
        }

        protected override async Task<bool> ExecuteQuery(NpgsqlCommand command)
        {
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }
    }
}
