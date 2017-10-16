using Exodus.Core;
using Exodus.Core.Parsers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Unit.RunMigrations.Default
{
    public class Tests
    {
        [Fact]
        public async Task RunMigrationsWithDefaultSetup_EnsureDefaultSetupIsRunFromCurrentDirectory()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            var currentDirectory = Directory.GetCurrentDirectory();
            directoryParser
                .Setup(parser => parser.Parse(currentDirectory))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator.MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(2, database.RunMigrationCounter);

            var appliedMigration = database.AppliedMigrations[0];
            Assert.Equal(1, appliedMigration.Version);
            Assert.Equal("TestMigration 01", appliedMigration.Name);
            Assert.Equal("-- Test migration 01", appliedMigration.Script);

            appliedMigration = database.AppliedMigrations[1];
            Assert.Equal(2, appliedMigration.Version);
            Assert.Equal("TestMigration 02", appliedMigration.Name);
            Assert.Equal("-- Test migration 02", appliedMigration.Script);

            directoryParser.Verify();
        }
    }
}
