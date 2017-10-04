using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Unit.Logging
{
    public class Tests
    {
        [Fact]
        public async Task TwoMigrations_OnEmptyExistingDatabase_WithoutAdditionalOperations()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            var logs = new List<string>();

            await migrator
                .Log(message => logs.Add(message))
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(2, database.RunMigrationCounter);

            Assert.Equal(4, logs.Count);
            Assert.Equal("Apply migrations:", logs[0]);
            Assert.Equal("1 - TestMigration 01", logs[1]);
            Assert.Equal("2 - TestMigration 02", logs[2]);
            Assert.Equal("2 migrations applied", logs[3]);

            directoryParser.Verify();
        }

        [Fact]
        public async Task TwoMigrations_OnEmptyExistingDatabase_WithCreateIfNotExists()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            var logs = new List<string>();

            await migrator
                .CreateDatabaseIfNotExists()
                .Log(message => logs.Add(message))
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(2, database.RunMigrationCounter);

            Assert.Equal(5, logs.Count);
            Assert.Equal("Create database if not exists: DatabaseMock", logs[0]);
            Assert.Equal("Apply migrations:", logs[1]);
            Assert.Equal("1 - TestMigration 01", logs[2]);
            Assert.Equal("2 - TestMigration 02", logs[3]);
            Assert.Equal("2 migrations applied", logs[4]);

            directoryParser.Verify();
        }

        [Fact]
        public async Task TwoMigrations_OnEmptyExistingDatabase_WithDropCreate()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            var logs = new List<string>();

            await migrator
                .DropCreateDatabase()
                .Log(message => logs.Add(message))
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(1, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(2, database.RunMigrationCounter);

            Assert.Equal(5, logs.Count);
            Assert.Equal("Drop and create database: DatabaseMock", logs[0]);
            Assert.Equal("Apply migrations:", logs[1]);
            Assert.Equal("1 - TestMigration 01", logs[2]);
            Assert.Equal("2 - TestMigration 02", logs[3]);
            Assert.Equal("2 migrations applied", logs[4]);

            directoryParser.Verify();
        }
    }
}
