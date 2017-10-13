using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Npgsql.Commands
{
    class KillAllConnections : PostgresCommand
    {
        public KillAllConnections(string connectionString, string databaseName)
            : base(connectionString)
        {
            Sql = $@"
                SELECT  pg_terminate_backend(pg_stat_activity.pid)
                FROM    pg_stat_activity
                WHERE   pg_stat_activity.datname = '{databaseName}'
                AND     pid <> pg_backend_pid();
            ";
        }
    }
}
