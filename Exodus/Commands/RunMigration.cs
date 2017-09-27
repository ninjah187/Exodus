using Exodus.Communication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Exodus.Commands
{
    class RunMigration : Command
    {
        readonly Migration _migration;

        public RunMigration(string connectionString, Migration migration)
            : base(connectionString)
        {
            _migration = migration;
            Sql = $@"
                {migration.Script}
                INSERT INTO Migrations VALUES (@version, @appliedOn, @name);
            ";
        }

        protected override void AddParameters(SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter("version", _migration.Version));
            parameters.Add(new SqlParameter("appliedOn", DateTime.Now));
            parameters.Add(new SqlParameter("name", _migration.Name));
        }
    }
}
