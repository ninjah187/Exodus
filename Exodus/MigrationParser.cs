using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    class MigrationParser
    {
        public async Task<Migration> Parse(string scriptFilePath)
        {
            var migrationId = ParseScriptFilePath(scriptFilePath);
            var sql = await File.ReadAllTextAsync(scriptFilePath);
            return new Migration(migrationId.version, migrationId.name, sql);
        }

        private (int version, string name) ParseScriptFilePath(string scriptFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(scriptFilePath);
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
