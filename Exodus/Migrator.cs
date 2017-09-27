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
            _pipeline.Setup.Add(() => drop.Execute());
            _pipeline.Setup.Add(() => create.Execute());
            return this;
        }

        public Migrator CreateDatabaseIfNotExists()
        {
            ValidateExtendedConfiguration(nameof(CreateDatabaseIfNotExists));
            var create = new CreateDatabaseIfNotExists(_configuration.ServerConnectionString, _configuration.DatabaseName);
            _pipeline.Setup.Add(() => create.Execute());
            return this;
        }

        public async Task Migrate()
        {
            foreach (var middleware in _pipeline.Setup)
            {
                await middleware();
            }
            await CreateMigrationsTableIfNotExists();
            await BuildMigrationsPipeline();
            foreach (var middleware in _pipeline.Migrations)
            {
                await middleware();
            }
        }

        private async Task BuildMigrationsPipeline()
        {
            _pipeline.Migrations.Clear();
            var projectDirectory = Directory.GetCurrentDirectory();
            var parseMigrationTasks = Directory
                .EnumerateFiles(projectDirectory, "*.sql", SearchOption.AllDirectories)
                .Select(async scriptFilePath => await _migrationParser.Parse(scriptFilePath))
                .ToArray();
            var appliedVersions = await GetAppliedVersions();
            var migrationsPipeline = (await Task.WhenAll(parseMigrationTasks))
                .OrderBy(migration => migration.Version)
                .Where(migration => !appliedVersions.Contains(migration.Version))
                .Select(migration => new RunMigration(_configuration.DatabaseConnectionString, migration))
                .Select(command => (Func<Task>)(() => command.Execute()));
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
    }
}
