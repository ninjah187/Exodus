using Exodus.Core.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Integration.Parsing.Directory
{
    public class Tests
    {
        [Fact]
        public async Task WithThreeMigrationsIncludingNestedOne()
        {
            var parser = new DefaultDirectoryParser();

            var tasks = parser.Parse("../../../Migrations");
            var migrations = await Task.WhenAll(tasks);

            Assert.Equal(3, migrations.Length);
            Assert.True(migrations.Any(migration => migration.Version == 1 &&
                                                    migration.Name == "TestMigration 01" &&
                                                    migration.Script == "-- Test migration 01"));
            Assert.True(migrations.Any(migration => migration.Version == 2 &&
                                                    migration.Name == "TestMigration 02" &&
                                                    migration.Script == "-- Test migration 02"));
            Assert.True(migrations.Any(migration => migration.Version == 3 &&
                                                    migration.Name == "NestedMigration" &&
                                                    migration.Script == "-- Test migration 03"));
        }

        [Fact]
        public async Task FromNotExistingDirectory()
        {
            var parser = new DefaultDirectoryParser();

            Func<Task> act = async () =>
            {
                var tasks = parser.Parse("NotExistingDirectory");
                var migrations = await Task.WhenAll(tasks);
            };

            await Assert.ThrowsAsync<DirectoryNotFoundException>(act);
        }
    }
}
