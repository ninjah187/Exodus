using Exodus.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Integration.Parsing.Assembly
{
    public class Tests
    {
        [Fact]
        public async Task WithTwoMigrations()
        {
            var parser = new DefaultAssemblyParser();
            
            var tasks = parser.Parse(new AssemblyName("Exodus.Tests.Migrations"));
            var migrations = await Task.WhenAll(tasks);

            Assert.Equal(2, migrations.Length);
            Assert.True(migrations.Any(migration => migration.Version == 1 &&
                                                    migration.Name == "TestMigration 01" &&
                                                    migration.Script == "-- Test migration 01"));
            Assert.True(migrations.Any(migration => migration.Version == 2 &&
                                                    migration.Name == "TestMigration 02" &&
                                                    migration.Script == "-- Test migration 02"));
        }
    }
}
