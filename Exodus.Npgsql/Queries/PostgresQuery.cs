using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Exodus.Core.Database.Communication;

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
