using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.CreateIfNotExists
{
    public class Tests
    {
        [Fact]
        public async Task CreateIfNotExists_WhenNotExists_Created()
        {
            var database = new DatabaseMock();
            var migrator = new Migrator(database);

            await migrator
                .CreateDatabaseIfNotExists()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrationVersions.Length);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
        }

        [Fact]
        public async Task CreateIfNotExists_WhenExists_Created()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var migrator = new Migrator(database);

            await migrator
                .CreateDatabaseIfNotExists()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrationVersions.Length);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
        }
    }
}
