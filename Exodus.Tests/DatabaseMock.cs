using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Tests
{
    public class DatabaseMock : IDatabase
    {
        public int CreateIfNotExistsCounter { get; set; }
        public int CreateMigrationsTableIfNotExistsCounter { get; set; }
        public int DropIfExistsCounter { get; set; }
        public int GetAppliedMigrationVersionsCounter { get; set; }
        public int RunMigrationCounter { get; set; }

        public bool DatabaseExists { get; set; }
        public bool MigrationsTableExists { get; set; }
        public int[] AppliedMigrationVersions { get; set; } = new int[0];
        public List<Migration> AppliedMigrations { get; set; } = new List<Migration>();

        public string Name => "DatabaseMock";

        public Task CreateIfNotExists()
        {
            CreateIfNotExistsCounter++;
            DatabaseExists = true;
            return Task.CompletedTask;
        }

        public Task CreateMigrationsTableIfNotExists()
        {
            CreateMigrationsTableIfNotExistsCounter++;
            MigrationsTableExists = true;
            return Task.CompletedTask;
        }

        public Task DropIfExists()
        {
            DropIfExistsCounter++;
            DatabaseExists = false;
            MigrationsTableExists = false;
            return Task.CompletedTask;
        }

        public Task<int[]> GetAppliedMigrationVersions()
        {
            GetAppliedMigrationVersionsCounter++;
            return Task.FromResult(AppliedMigrationVersions);
        }

        public Task RunMigration(Migration migration)
        {
            RunMigrationCounter++;
            AppliedMigrations.Add(migration);
            return Task.CompletedTask;
        }
    }
}
