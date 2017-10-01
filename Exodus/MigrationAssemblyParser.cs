using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    class MigrationAssemblyParser
    {
        public IEnumerable<Task<Migration>> Parse(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return assembly
                .GetManifestResourceNames()
                .Select(name => new
                {
                    Name = name,
                    Stream = assembly.GetManifestResourceStream(name)
                })
                .Select(async resource =>
                {
                    var migrationId = ParseResourceName(assemblyName, resource.Name);
                    using (var reader = new StreamReader(resource.Stream))
                    {
                        var sql = await reader.ReadToEndAsync();
                        return new Migration(migrationId.version, migrationId.name, sql);
                    }
                });
        }

        private (int version, string name) ParseResourceName(AssemblyName assemblyName, string resourceName)
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
