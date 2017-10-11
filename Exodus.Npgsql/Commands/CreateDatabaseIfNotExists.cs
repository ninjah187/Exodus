using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Exodus.Npgsql.Commands
{
    class CreateDatabaseIfNotExists : Command
    {
        readonly string _databaseName;

        public CreateDatabaseIfNotExists(string connectionString, string databaseName)
            : base(connectionString, false)
        {
            _databaseName = databaseName;
            Sql = $@"
                CREATE DATABASE IF NOT EXISTS @databaseName
                WITH OWNER = 'postgres'
                ENCODING = 'UTF8'
                CONNECTION LIMIT = -1;
            ";
        }

        protected override void AddParameters(NpgsqlParameterCollection parameters)
        {
            parameters.Add(new NpgsqlParameter("databaseName", _databaseName));
        }
    }
}
