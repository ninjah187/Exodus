using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Parsers
{
    public class DefaultDirectoryParser : IDirectoryParser
    {
        public IEnumerable<Task<Migration>> Parse(string directoryPath)
        {
            return Directory
                .EnumerateFiles(directoryPath, "*.sql", SearchOption.AllDirectories)
                .Select(scriptFilePath => ParseFile(scriptFilePath));
        }

        private async Task<Migration> ParseFile(string scriptFilePath)
        {
            var migrationId = ParseFilePath(scriptFilePath);
            var sql = await File.ReadAllTextAsync(scriptFilePath);
            return new Migration(migrationId.version, migrationId.name, sql);
        }

        private (int version, string name) ParseFilePath(string migrationFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(migrationFilePath);
            var tokens = fileName
                .Split("-")
                .Select(token => token.Trim())
                .ToArray();
            var version = int.Parse(tokens[0]);
            var name = tokens[1];
            return (version, name);
        }
    }
}
