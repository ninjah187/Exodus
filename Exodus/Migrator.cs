using Exodus.Commands;
using Exodus.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    public class Migrator
    {
        readonly MigratorConfiguration _configuration;
        readonly MigrationParser _migrationParser;
        readonly MigratorPipeline _pipeline;
        Action<string> _log;

        public Migrator(MigratorConfiguration configuration)
        {
            _configuration = configuration;
            _migrationParser = new MigrationParser();
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
            Log("Work completed");
        }

        public void Migrate()
            => Task.WaitAll(MigrateAsync());

        private async Task BuildMigrationsPipeline()
        {
            _pipeline.Migrations.Clear();
            var projectDirectory = Directory.GetCurrentDirectory();
            var parseMigrationTasks = Directory
                .EnumerateFiles(projectDirectory, "*.sql", SearchOption.AllDirectories)
                .Select(scriptFilePath => _migrationParser.Parse(scriptFilePath))
                .ToArray();
            var appliedVersions = await GetAppliedVersions();
            var migrationsPipeline = (await Task.WhenAll(parseMigrationTasks))
                .OrderBy(migration => migration.Version)
                .Where(migration => !appliedVersions.Contains(migration.Version))
                .Select(migration => new RunMigration(_configuration.DatabaseConnectionString, migration))
                .Select(command => (Func<Task>) (async () =>
                {
                    await command.Execute();
                    Log($"{command.Migration.Version} - {command.Migration.Name}");
                }));
            _pipeline.Migrations.AddRange(migrationsPipeline);
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
