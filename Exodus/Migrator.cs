using Exodus.Commands;
using Exodus.Queries;
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
        readonly MigratorConfiguration _configuration;
        readonly MigrationDirectoryParser _migrationDirectoryParser;
        readonly MigrationAssemblyParser _migrationAssemblyParser;
        readonly MigratorPipeline _pipeline;
        string _migrationsDirectoryPath;
        AssemblyName _migrationsAssemblyName;
        Action<string> _log;

        public Migrator(string connectionString)
            : this(new MigratorConfiguration(connectionString))
        {
        }

        public Migrator(MigratorConfiguration configuration)
        {
            _configuration = configuration;
            _migrationDirectoryParser = new MigrationDirectoryParser();
            _migrationAssemblyParser = new MigrationAssemblyParser();
            _pipeline = new MigratorPipeline();
        }

        public Migrator DropCreateDatabase()
        {
            ValidateExtendedConfiguration(nameof(DropCreateDatabase));
            var drop = new DropDatabaseIfExists(_configuration.ServerConnectionString, _configuration.DatabaseName);
            var create = new CreateDatabaseIfNotExists(_configuration.ServerConnectionString, _configuration.DatabaseName);
            _pipeline.Setup.Add(async () => Log($"Drop and create database: {_configuration.DatabaseName}"));
            _pipeline.Setup.Add(() => drop.Execute());
            _pipeline.Setup.Add(() => create.Execute());
            return this;
        }

        public Migrator CreateDatabaseIfNotExists()
        {
            ValidateExtendedConfiguration(nameof(CreateDatabaseIfNotExists));
            var create = new CreateDatabaseIfNotExists(_configuration.ServerConnectionString, _configuration.DatabaseName);
            _pipeline.Setup.Add(async () => Log($"Create database if not exists: {_configuration.DatabaseName}"));
            _pipeline.Setup.Add(() => create.Execute());
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
            await CreateMigrationsTableIfNotExists();
            await BuildMigrationsPipeline();
            Log("Run migrations:");
            foreach (var middleware in _pipeline.Migrations)
            {
                await middleware();
            }
            Log("Completed");
        }

        public void Migrate()
            => MigrateAsync().Wait();

        private async Task BuildMigrationsPipeline()
        {
            _pipeline.Migrations.Clear();
            var parseMigrationTasks = ParseMigrations();
            var appliedVersions = await GetAppliedVersions();
            var migrationsPipeline = (await Task.WhenAll(parseMigrationTasks))
                .Where(migration => !appliedVersions.Contains(migration.Version))
                .Select(migration => new RunMigration(_configuration.DatabaseConnectionString, migration))
                .GroupBy(command => command.Migration.Version)
                .Select(group => group.First())
                .OrderBy(command => command.Migration.Version)
                .Select(command => (Func<Task>) (async () =>
                {
                    await command.Execute();
                    Log($"{command.Migration.Version} - {command.Migration.Name}");
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
                    $"Assembly {_migrationsAssemblyName.FullName} not found." +
                    $"Check if migrations project is referenced and " +
                    $"consider using full assembly name.");
            }
        }

        private async Task<int[]> GetAppliedVersions()
        {
            var get = new GetAppliedMigrationVersions(_configuration.DatabaseConnectionString);
            return await get.Execute();
        }

        private async Task CreateMigrationsTableIfNotExists()
        {
            var create = new CreateMigrationsTableIfNotExists(_configuration.DatabaseConnectionString);
            await create.Execute();
        }

        private void ValidateExtendedConfiguration(string operationName)
        {
            if (!string.IsNullOrEmpty(_configuration.ServerConnectionString) &&
                !string.IsNullOrEmpty(_configuration.DatabaseName))
            {
                return;
            }
            throw new InvalidOperationException(
                $"In order to execute operation {operationName}, " +
                $"you need to provide {nameof(MigratorConfiguration.ServerConnectionString)} and " +
                $"{nameof(MigratorConfiguration.DatabaseName)} in migrator's configuration.");
        }

        private void Log(string message)
            => _log?.Invoke(message);
    }
}
