using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Npgsql.Commands
{
    class CreateDatabase : Command
    {
        public CreateDatabase(string connectionString, string databaseName)
            : base(connectionString, false)
        {
            Sql = $@"
                CREATE DATABASE ""{databaseName}""
                WITH OWNER = postgres
                ENCODING = 'UTF8'
                CONNECTION LIMIT = -1;
            ";
        }
    }
}
