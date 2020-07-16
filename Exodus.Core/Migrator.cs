using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Exodus.Core.Parsers;
using Exodus.Core.Database;

namespace Exodus.Core
{
    public class Migrator
    {
        readonly IDatabase          _database;
        readonly IDirectoryParser   _directoryParser;
        readonly IAssemblyParser    _assemblyParser;
        readonly Pipeline           _pipeline;

        string          _directoryPath;
        AssemblyName    _assemblyName;
        Action<string>  _log;

        public Migrator(IDatabase database)
            : this(database,
                   new DefaultDirectoryParser(),
                   new DefaultAssemblyParser())
        {
        }

        public Migrator(
            IDatabase database,
            IDirectoryParser directoryParser,
            IAssemblyParser assemblyParser)
        {
            _database = database;
            _directoryParser = directoryParser;
            _assemblyParser = assemblyParser;
            _pipeline = new Pipeline();
        }

        public Migrator DropDatabase()
        {
            _pipeline.Setup.Add(async () => Log($"Drop database: {_database.Name}"));
            _pipeline.Setup.Add(() => _database.DropIfExists());
            return this;
        }

        public Migrator DropCreateDatabase()
        {
            _pipeline.Setup.Add(async () => Log($"Drop and create database: {_database.Name}"));
            _pipeline.Setup.Add(() => _database.DropIfExists());
            _pipeline.Setup.Add(() => _database.CreateIfNotExists());
            return this;
        }

        public Migrator CreateDatabaseIfNotExists()
        {
            _pipeline.Setup.Add(async () => Log($"Create database if not exists: {_database.Name}"));
            _pipeline.Setup.Add(() => _database.CreateIfNotExists());
            return this;
        }

        public Migrator Log(Action<string> log)
        {
            _log = log;
            return this;
        }

        public Migrator LogToConsole()
        {
            _log = message => Console.WriteLine(message);
            return this;
        }

        public Migrator FromAssembly(string assemblyName)
            => FromAssembly(new AssemblyName(assemblyName));

        public Migrator FromAssembly(AssemblyName assemblyName)
        {
            _assemblyName = assemblyName;
            return this;
        }

        public Migrator FromDirectory(string directoryPath)
        {
            _directoryPath = directoryPath;
            return this;
        }

        public async Task MigrateAsync()
        {
            foreach (var middleware in _pipeline.Setup)
            {
                await middleware();
            }
            await _database.CreateMigrationsTableIfNotExists();
            await BuildMigrationsPipeline();
            LogApplyMigrations();
            foreach (var middleware in _pipeline.Migrations)
            {
                await middleware();
            }
            Log($"{_pipeline.Migrations.Count} migrations applied");
        }

        public void Migrate()
            => MigrateAsync().Wait();

        private async Task BuildMigrationsPipeline()
        {
            _pipeline.Migrations.Clear();
            var parseMigrationTasks = ParseMigrations();
            var appliedVersions = await _database.GetAppliedMigrationVersions();
            var migrationsPipeline = (await Task.WhenAll(parseMigrationTasks))
                .GroupBy(migration => migration.Version)
                .Select(group => group.First())
                .Where(migration => !appliedVersions.Contains(migration.Version))
                .OrderBy(migration => migration.Version)
                .Select(migration => (Func<Task>) (async () =>
                {
                    await _database.RunMigration(migration);
                    Log($"{migration.Version} - {migration.Name}");
                }));
            _pipeline.Migrations.AddRange(migrationsPipeline);
        }

        private IEnumerable<Task<Migration>> ParseMigrations()
        {
            try
            {
                if (_assemblyName == null)
                {
                    _directoryPath = _directoryPath ?? Directory.GetCurrentDirectory();
                    return _directoryParser.Parse(_directoryPath);
                }
                return _assemblyParser.Parse(_assemblyName);
            }
            catch (DirectoryNotFoundException exception)
            {
                throw new DirectoryNotFoundException(
                    $"Directory {_directoryPath} was not found. " +
                    $"Check correctness of target path.",
                    exception);
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException(
                    $"Assembly {_assemblyName.FullName} was not found." +
                    $"Check if migrations project is referenced. " +
                    $"Consider using full assembly name.",
                    exception);
            }
        }

        private void LogApplyMigrations()
        {
            var source = _assemblyName == null
                ? $"directory {_directoryPath}"
                : $"assembly {_assemblyName.Name}";
            Log($"Apply migrations from {source}:");
        }

        private void Log(string message)
            => _log?.Invoke(message);
    }
}
