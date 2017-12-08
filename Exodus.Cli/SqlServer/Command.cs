using Exodus.Cli.Models;
using Exodus.Cli.Services;
using Exodus.Core;
using Jarilo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.SqlServer
{
    [Command("sql", "Perform migrations on SQL Server database.")]
    [View(typeof(View))]
    class Command
    {
        readonly IMigratorFactory<SqlServerMigrator> _migratorFactory;

        public Command(IMigratorFactory<SqlServerMigrator> migratorFactory)
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
