using System;
using System.Collections.Generic;
using System.Text;
using Exodus.Core;
using Exodus.Cli.Models;

namespace Exodus.Cli.Services
{
    class NpgsqlMigratorFactory : IMigratorFactory<NpgsqlMigrator>
    {
        public Migrator CreateMigrator(
            string connectionString,
            string directoryPath,
            DatabaseSetupOperation databaseSetupOperation,
            Action<string> log = null)
        {
            log = log ?? (_ => {});
            var migrator = new NpgsqlMigrator(connectionString)
                .FromDirectory(directoryPath)
                .SetupDatabase(databaseSetupOperation)
                .Log(log);
            return migrator;
        }
    }
}
