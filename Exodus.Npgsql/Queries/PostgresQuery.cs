using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Exodus.Database.Communication;
using Npgsql;

namespace Exodus.Npgsql.Queries
{
    abstract class PostgresQuery<TResult> : Query<NpgsqlConnection, NpgsqlCommand, NpgsqlParameterCollection, TResult>
    {
        public PostgresQuery(string connectionString)
            : base(connectionString)
        {
        }

        protected override NpgsqlConnection CreateConnection()
            => new NpgsqlConnection(ConnectionString);

        protected override NpgsqlCommand CreateCommand(NpgsqlConnection connection)
            => new NpgsqlCommand(Sql, connection);

        protected override Task OpenConnection(NpgsqlConnection connection)
            => connection.OpenAsync();
    }
}
