using Exodus.Communication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Queries
{
    class CheckIfDatabaseExists : Query<bool>
    {
        readonly string _databaseName;

        public CheckIfDatabaseExists(string connectionString, string databaseName)
            : base(connectionString)
        {
            _databaseName = databaseName;
            Sql = "SELECT db_id(@databaseName)";
        }

        protected override void AddParameters(SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter("databaseName", _databaseName));
        }

        protected override async Task<bool> ExecuteQuery(SqlCommand command)
        {
            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value;
        }
    }
}
