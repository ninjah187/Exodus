using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Exodus.SqlServer.Commands
{
    class CreateDatabaseIfNotExists : Command
    {
        readonly string _databaseName;

        public CreateDatabaseIfNotExists(string connectionString, string databaseName)
            : base(connectionString)
        {
            _databaseName = databaseName;
            Sql = $@"
                IF (db_id(@databaseName) IS NULL)
                BEGIN
                    CREATE DATABASE [{databaseName}];
                END
            ";
        }

        protected override void AddParameters(SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter("databaseName", _databaseName));
        }
    }
}
