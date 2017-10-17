using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Core.Database
{
    public interface IDatabase
    {
        string Name { get; }
        Task DropIfExists();
        Task CreateIfNotExists();
        Task CreateMigrationsTableIfNotExists();
        Task RunMigration(Migration migration);
        Task<int[]> GetAppliedMigrationVersions();
    }
}
