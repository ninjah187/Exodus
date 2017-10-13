using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Npgsql.Commands
{
    class CreateMigrationsTableIfNotExists : PostgresCommand
    {
        public CreateMigrationsTableIfNotExists(string connectionString)
            : base(connectionString)
        {
            Sql = $@"
                CREATE TABLE IF NOT EXISTS Migrations (
                    Version     integer NOT NULL,
                    AppliedOn   date NOT NULL,
                    Name        varchar(1024) NOT NULL);
            ";
        }
    }
}
