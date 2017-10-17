using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Exodus.Npgsql;

namespace Exodus
{
    public class NpgsqlMigrator : Migrator
    {
        public NpgsqlMigrator(string connectionString)
            : base(new NpgsqlDatabase(connectionString))
        {
        }
    }
}
