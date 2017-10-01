using Exodus.Communication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Exodus.Commands
{
    class RunMigration : Command
    {
        public Migration Migration { get; }

        public RunMigration(string connectionString, Migration migration)
            : base(connectionString)
        {
            Migration = migration;
            Sql = $@"
                {migration.Script}
                INSERT INTO Migrations VALUES (@version, @appliedOn, @name);
            ";
        }

        protected override void AddParameters(SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter("version", Migration.Version));
            parameters.Add(new SqlParameter("appliedOn", DateTime.Now));
            parameters.Add(new SqlParameter("name", Migration.Name));
        }
    }
}
