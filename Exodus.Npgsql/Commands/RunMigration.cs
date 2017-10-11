using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Exodus.Npgsql.Commands
{
    class RunMigration : Command
    {
        readonly Migration _migration;

        public RunMigration(string connectionString, Migration migration)
            : base(connectionString)
        {
            _migration = migration;
            Sql = $@"
            
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
