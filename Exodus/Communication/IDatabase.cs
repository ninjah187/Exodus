using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Communication
{
    interface IDatabase
    {
        string Name { get; }
        Task DropIfExists();
        Task CreateIfNotExists();
        Task CreateMigrationsTableIfNotExists();
        Task RunMigration(Migration migration);
        Task<int[]> GetAppliedMigrationVersions();
    }
}
