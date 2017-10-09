using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Parsers
{
    public class DefaultAssemblyParser : IAssemblyParser
    {
        public IEnumerable<Task<Migration>> Parse(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return assembly
                .GetManifestResourceNames()
                .Select(async name =>
                {
                    var migrationId = ParseResourceName(name);
                    var stream = assembly.GetManifestResourceStream(name);
                    using (var reader = new StreamReader(stream))
                    {
                        var sql = await reader.ReadToEndAsync();
                        return new Migration(migrationId.version, migrationId.name, sql);
                    }
                });
        }

        private (int version, string name) ParseResourceName(string resourceName)
        {
            var tokens = resourceName
                .Split("-")
                .Select(token => token.Trim())
                .ToArray();
            var version = int.Parse(tokens[0].Split(".").Last());
            var name = tokens[1].Split(".").First();
            return (version, name);
        }
    }
}
