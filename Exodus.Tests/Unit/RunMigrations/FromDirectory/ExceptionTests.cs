using Exodus.Core;
using Exodus.Core.Parsers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.Unit.RunMigrations.FromDirectory
{
    public class ExceptionTests
    {
        [Fact]
        public async Task FromNotExistingDirectory_ThrowsDirectoryNotFoundException()
        {
            var database = new DatabaseMock();
            var notExistingDirectoryPath = "NotExistingDirectory";
            var directoryParser = new Mock<IDirectoryParser>();
            directoryParser
                .Setup(parser => parser.Parse(notExistingDirectoryPath))
                .Throws<DirectoryNotFoundException>();
            var migrator = new Migrator(database, directoryParser.Object, null);

            Func<Task> act = async () =>
            {
                await migrator
                    .FromDirectory(notExistingDirectoryPath)
                    .MigrateAsync();
            };

            await Assert.ThrowsAsync<DirectoryNotFoundException>(act);
            directoryParser.VerifyAll();
        }
    }
}
