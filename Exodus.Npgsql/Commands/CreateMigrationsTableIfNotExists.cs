using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Npgsql.Commands
{
    class CreateMigrationsTableIfNotExists : Command
    {
        public CreateMigrationsTableIfNotExists(string connectionString)
            : base(connectionString)
        {
            Sql = $@"
                DO
                $do$
                BEGIN
                    IF NOT EXISTS (
                        SELECT  1
                        FROM    pg_catalog.pg_class c
                        JOIN    pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                        WHERE   n.nspname = ANY(current_schemas(FALSE))
                        AND     n.nspname NOT LIKE 'pg_%'                              -- exclude system schemas
                        AND     c.relname = 'Migrations'
                        AND     c.relkind = 'r')
                    THEN
                        CREATE TABLE Migrations (
                            Version     integer NOT NULL,
                            AppliedOn   date NOT NULL,
                            Name        varchar(1024) NOT NULL);
                    END IF;
                END
                $do$;
            ";
        }
    }
}
