using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Exodus.Npgsql.Commands
{
    class RunMigration : PostgresCommand
    {
        readonly Migration _migration;

        public RunMigration(string databaseConnectionString, Migration migration)
            : base(databaseConnectionString)
        {
            _migration = migration;
            Sql = $@"
                {migration.Script}
                INSERT INTO Migrations VALUES (@version, @appliedOn, @name);
            ";
        }

        protected override void AddParameters(NpgsqlParameterCollection parameters)
        {
            parameters.Add(new NpgsqlParameter("version", _migration.Version));
            parameters.Add(new NpgsqlParameter("appliedOn", DateTime.Now));
            parameters.Add(new NpgsqlParameter("name", _migration.Name));
        }
    }
}
