using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Exodus.Npgsql.Commands
{
    class DropDatabaseIfExists : Command
    {
        readonly string _databaseName;

        public DropDatabaseIfExists(string connectionString, string databaseName)
            : base(connectionString, false)
        {
            _databaseName = databaseName;
            Sql = "DROP DATABASE IF EXISTS @databaseName";
        }

        protected override void AddParameters(NpgsqlParameterCollection parameters)
        {
            parameters.Add(new NpgsqlParameter("databaseName", _databaseName));
        }
    }
}
