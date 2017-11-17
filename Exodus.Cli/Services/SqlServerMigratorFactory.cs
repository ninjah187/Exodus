using Exodus.Cli.Models;
using Exodus.Cli.Npgsql;
using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Services
{
    class SqlServerMigratorFactory : IMigratorFactory<SqlServerMigrator>
    {
        public Migrator CreateMigrator(
            string connectionString,
            string directoryPath,
            DatabaseSetupOperation databaseSetupOperation,
            Action<string> log = null)
        {
            log = log ?? (_ => {});
            var migrator = new SqlServerMigrator(connectionString)
                .FromDirectory(directoryPath)
                .SetupDatabase(databaseSetupOperation)
                .Log(log);
            return migrator;
        }
    }
}
