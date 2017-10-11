using System;
using System.Collections.Generic;
using System.Text;
using Exodus.SqlServer;
using Exodus.Core;

namespace Exodus
{
    public class SqlServerMigrator : Migrator
    {
        public SqlServerMigrator(string databaseConnectionString)
            : base(new SqlServerDatabase(databaseConnectionString))
        {
        }
    }
}
