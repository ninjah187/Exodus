using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.DropCreate
{
    public class Tests
    {
        [Fact]
        public async Task DropCreate_WhenNotExists_DroppedAndCreated()
        {
            var database = new DatabaseMock();
            var migrator = new Migrator(database);

            await migrator
                .DropCreateDatabase()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrationVersions.Length);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(1, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
        }

        [Fact]
        public async Task DropCreate_WhenExists_DroppedAndCreated()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var migrator = new Migrator(database);

            await migrator
                .DropCreateDatabase()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrationVersions.Length);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(1, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
        }
    }
}
