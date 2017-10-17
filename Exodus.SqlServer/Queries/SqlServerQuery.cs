using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Exodus.Core.Database.Communication;

namespace Exodus.SqlServer.Queries
{
    abstract class SqlServerQuery<TResult> : Query<SqlConnection, SqlCommand, SqlParameterCollection, TResult>
    {
        public SqlServerQuery(string connectionString) 
            : base(connectionString)
        {
        }

        protected override SqlConnection CreateConnection()
            => new SqlConnection(ConnectionString);

        protected override SqlCommand CreateCommand(SqlConnection connection)
            => new SqlCommand(Sql, connection);

        protected override Task OpenConnection(SqlConnection connection)
            => connection.OpenAsync();
    }
}
