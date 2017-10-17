using Exodus.Core;
using Exodus.Core.Parsers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Unit.RunMigrations.FromDirectory
{
    public class Tests
    {
        [Fact]
        public async Task RunMigrationsFromDirectory_OnEmptyExistingDatabase_AllMigrationsApplied()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse("MockMigrationsDirectory"))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);
            
            await migrator
                .FromDirectory("MockMigrationsDirectory")
                .MigrateAsync();

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

        [Fact]
        public async Task OnDatabaseWithAppliedMigration_HigherMigrationApplied()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(1, "TestMigration 01", "-- Test migration 01"));
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse("MockMigrationsDirectory"))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator
                .FromDirectory("MockMigrationsDirectory")
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(1, database.RunMigrationCounter);

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

        [Fact]
        public async Task OnDatabaseWithAppliedMigration_LowerMigrationApplied()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(2, "TestMigration 02", "-- Test migration 02"));
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse("MockMigrationsDirectory"))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator
                .FromDirectory("MockMigrationsDirectory")
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(1, database.RunMigrationCounter);

            var appliedMigration = database.AppliedMigrations[0];
            Assert.Equal(2, appliedMigration.Version);
            Assert.Equal("TestMigration 02", appliedMigration.Name);
            Assert.Equal("-- Test migration 02", appliedMigration.Script);

            appliedMigration = database.AppliedMigrations[1];
            Assert.Equal(1, appliedMigration.Version);
            Assert.Equal("TestMigration 01", appliedMigration.Name);
            Assert.Equal("-- Test migration 01", appliedMigration.Script);

            directoryParser.Verify();
        }

        [Fact]
        public async Task OnDatabaseWithAllMigrationsApplied_NoneMigrationsApplied()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(1, "TestMigration 01", "-- Test migration 01"));
            database.AppliedMigrations.Add(new Migration(2, "TestMigration 02", "-- Test migration 02"));
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse("MockMigrationsDirectory"))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                })
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator
                .FromDirectory("MockMigrationsDirectory")
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(0, database.RunMigrationCounter);

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
