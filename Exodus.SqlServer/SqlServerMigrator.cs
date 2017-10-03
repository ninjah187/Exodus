using System;
using System.Collections.Generic;
using System.Text;
using Exodus.SqlServer;

namespace Exodus
{
    public class SqlServerMigrator : Migrator
    {
        public SqlServerMigrator(string connectionString)
            : base(new SqlServerDatabase(connectionString))
        {
        }
    }
}
