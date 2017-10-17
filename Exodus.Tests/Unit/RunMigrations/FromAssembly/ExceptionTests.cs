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

namespace Exodus.Tests.Unit.RunMigrations.FromAssembly
{
    public class ExceptionTests
    {
        [Fact]
        public async Task FromNotExistingAssembly_ThrowsFileNotFoundException()
        {
            var database = new DatabaseMock();
            var notExistingAssemblyName = new AssemblyName("NotExistingAssembly");
            var assemblyParser = new Mock<IAssemblyParser>();
            assemblyParser
                .Setup(parser => parser.Parse(notExistingAssemblyName))
                .Throws<FileNotFoundException>();
            var migrator = new Migrator(database, null, assemblyParser.Object);

            Func<Task> act = async () =>
            {
                await migrator
                    .FromAssembly(notExistingAssemblyName)
                    .MigrateAsync();
            };

            await Assert.ThrowsAsync<FileNotFoundException>(act);
            assemblyParser.VerifyAll();
        }
    }
}
