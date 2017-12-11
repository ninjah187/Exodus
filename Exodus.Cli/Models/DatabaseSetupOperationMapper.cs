using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Models
{
    static class DatabaseSetupOperationMapper
    {
        public static Migrator SetupDatabase(this Migrator migrator, DatabaseSetupOperation setupOperation)
        {
            var setup = setupOperation.Map();
            return setup(migrator);
        }

        public static Func<Migrator, Migrator> Map(this DatabaseSetupOperation operation)
        {
            switch (operation)
            {
                case DatabaseSetupOperation.CreateIfNotExists:
                    return migrator => migrator.CreateDatabaseIfNotExists();
                case DatabaseSetupOperation.DropCreate:
                    return migrator => migrator.DropCreateDatabase();
                case DatabaseSetupOperation.None:
                default:
                    return migrator => migrator;
            }
        }
    }
}
