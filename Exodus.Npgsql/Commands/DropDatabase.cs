﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Npgsql.Commands
{
    class DropDatabase : PostgresCommand
    {
        public DropDatabase(string serverConnectionString, string databaseName)
            : base(serverConnectionString, false)
        {
            Sql = $@"DROP DATABASE ""{databaseName}""";
        }
    }
}
