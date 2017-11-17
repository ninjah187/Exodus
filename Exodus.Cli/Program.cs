using Exodus.Cli.Services;
using Jarilo;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Exodus.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            app.Services
                .AddSingleton<IMigratorFactory<SqlServerMigrator>, SqlServerMigratorFactory>()
                .AddSingleton<IMigratorFactory<NpgsqlMigrator>, NpgsqlMigratorFactory>();
            app.ReadEvalPrintLoop();
            //app.Run(args);
        }
    }
}
