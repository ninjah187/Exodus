using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Exodus
{
    public class Migrator
    {
        readonly IDatabase _database;
        readonly MigrationDirectoryParser _migrationDirectoryParser;
        readonly MigrationAssemblyParser _migrationAssemblyParser;
        readonly MigratorPipeline _pipeline;
        string _migrationsDirectoryPath;
        AssemblyName _migrationsAssemblyName;
        Action<string> _log;

        public Migrator(IDatabase database)
        {
            _database = database;
            _migrationDirectoryParser = new MigrationDirectoryParser();
            _migrationAssemblyParser = new MigrationAssemblyParser();
            _pipeline = new MigratorPipeline();
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
            _migrationsAssemblyName = assemblyName;
            return this;
        }

        public Migrator FromDirectory(string migrationsDirectory)
        {
            _migrationsDirectoryPath = migrationsDirectory;
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
            Log("Apply migrations:");
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
            if (_migrationsAssemblyName == null)
            {
                var projectDirectory = _migrationsDirectoryPath ?? Directory.GetCurrentDirectory();
                return _migrationDirectoryParser.Parse(projectDirectory);
            }
            try
            {
                return _migrationAssemblyParser.Parse(_migrationsAssemblyName);
            }
            catch (FileNotFoundException)
            {
                throw new InvalidOperationException(
                    $"Assembly {_migrationsAssemblyName.FullName} not found. " +
                    $"Check if migrations project is referenced and " +
                    $"consider using full assembly name.");
            }
        }

        private void Log(string message)
            => _log?.Invoke(message);
    }
}
