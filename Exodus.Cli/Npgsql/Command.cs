using Exodus.Cli.Models;
using Exodus.Cli.Services;
using Exodus.Core;
using Jarilo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Npgsql
{
    [Command("npgsql", "Perform migrations on PostgreSQL database.")]
    class Command
    {
        readonly IMigratorFactory<NpgsqlMigrator> _migratorFactory;

        public Command(IMigratorFactory<NpgsqlMigrator> migratorFactory)
        {
            _migratorFactory = migratorFactory;
        }

        public ViewModel Run(Arguments arguments, Options options)
        {
            var logs = new List<string>();
            var migrator = _migratorFactory.CreateMigrator(
                arguments.ConnectionString,
                arguments.Directory,
                options.SetupOperation,
                log => logs.Add(log));
            var startTime = DateTime.Now;
            migrator.Migrate();
            var endTime = DateTime.Now;
            return new ViewModel
            {
                Logs = logs.ToArray(),
                StartTime = startTime,
                EndTime = endTime
            };
        }
    }
}
