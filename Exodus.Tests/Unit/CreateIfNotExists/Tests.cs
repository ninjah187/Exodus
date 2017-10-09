using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Reflection;
using System.IO;
using Exodus.Parsers;
using Exodus.Core;

namespace Exodus.Tests.Unit.CreateIfNotExists
{
    public class Tests
    {
        [Fact]
        public async Task CreateIfNotExists_WhenNotExists_Created()
        {
            var database = new DatabaseMock();
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[0])
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator
                .CreateDatabaseIfNotExists()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrations.Count);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(0, database.RunMigrationCounter);
            directoryParser.Verify();
        }

        [Fact]
        public async Task CreateIfNotExists_WhenExists_Created()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(Directory.GetCurrentDirectory()))
                .Returns(() => new Task<Migration>[0])
                .Verifiable();
            var migrator = new Migrator(database, directoryParser.Object, null);

            await migrator
                .CreateDatabaseIfNotExists()
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrations.Count);
            Assert.Equal(1, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(0, database.RunMigrationCounter);
            directoryParser.Verify();
        }
    }
}
