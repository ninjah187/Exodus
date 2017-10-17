using Exodus.Core;
using Exodus.Core.Parsers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            Assert.Equal("1 - TestMigration 01", logs[2]);
            Assert.Equal("2 - TestMigration 02", logs[3]);
            Assert.Equal("2 migrations applied", logs[4]);

            directoryParser.Verify();
        }

        [Fact]
        public async Task OneMigration_OnExistingDatabaseWithLowerMigration_WithoutAdditionalOperations()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(1, "TestMigration 01", "-- Test migration 01"));
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
            Assert.Equal(1, database.RunMigrationCounter);

            Assert.Equal(3, logs.Count);
            Assert.Equal("2 - TestMigration 02", logs[1]);
            Assert.Equal("1 migrations applied", logs[2]);

            directoryParser.Verify();
        }

        [Fact]
        public async Task OneMigration_OnExistingDatabaseWithHigherMigration_WithoutAdditionalOperations()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(2, "TestMigration 02", "-- Test migration 02"));
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
            Assert.Equal(1, database.RunMigrationCounter);

            Assert.Equal(3, logs.Count);
            Assert.Equal("1 - TestMigration 01", logs[1]);
            Assert.Equal("1 migrations applied", logs[2]);

            directoryParser.Verify();
        }

        [Fact]
        public async Task NoMigrations_OnExistingDatabaseWithAllMigrationsApplied_WithoutAdditionalOperations()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            database.AppliedMigrations.Add(new Migration(1, "TestMigration 01", "-- Test migration 01"));
            database.AppliedMigrations.Add(new Migration(2, "TestMigration 02", "-- Test migration 02"));
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[]
                {
                    Task.FromResult(new Migration(1, "TestMigration 01", "-- Test migration 01")),
                    Task.FromResult(new Migration(2, "TestMigration 02", "-- Test migration 02"))
                });
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
            Assert.Equal(0, database.RunMigrationCounter);

            Assert.Equal(2, logs.Count);
            Assert.Equal("0 migrations applied", logs[1]);

            directoryParser.VerifyAll();
        }

        [Fact]
        public async Task MigrationsFromDirectory_ApplyMigrationsLog()
        {
            var database = new DatabaseMock();
            var currentDirectory = Directory.GetCurrentDirectory();
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(currentDirectory))
                .Returns(() => new Task<Migration>[0]);
            var migrator = new Migrator(database, directoryParser.Object, null);

            var logs = new List<string>();

            await migrator
                .FromDirectory(currentDirectory)
                .Log(message => logs.Add(message))
                .MigrateAsync();

            Assert.Equal(2, logs.Count);
            Assert.Equal($"Apply migrations from directory {currentDirectory}:", logs[0]);
            directoryParser.VerifyAll();
        }

        [Fact]
        public async Task MigrationsFromAssembly_ApplyMigrationsLog()
        {
            var database = new DatabaseMock();
            var assemblyName = new AssemblyName("Test.Assembly");
            var assemblyParser = new Mock<IAssemblyParser>();
            assemblyParser
                .Setup(parser => parser.Parse(assemblyName))
                .Returns(() => new Task<Migration>[0]);
            var migrator = new Migrator(database, null, assemblyParser.Object);

            var logs = new List<string>();

            await migrator
                .FromAssembly(assemblyName)
                .Log(message => logs.Add(message))
                .MigrateAsync();

            Assert.Equal(2, logs.Count);
            Assert.Equal($"Apply migrations from assembly {assemblyName.Name}:", logs[0]);
            assemblyParser.VerifyAll();
        }
    }
}
