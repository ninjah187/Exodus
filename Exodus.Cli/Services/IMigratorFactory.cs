using Exodus.Cli.Models;
using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Services
{
    interface IMigratorFactory<TMigrator>
        where TMigrator : Migrator
    {
        Migrator CreateMigrator(
            string connectionString,
            string directoryPath,
            DatabaseSetupOperation databaseSetupOperation,
            Action<string> log = null);
    }
}
