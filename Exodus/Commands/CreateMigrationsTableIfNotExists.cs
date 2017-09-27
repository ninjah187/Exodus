using Exodus.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Commands
{
    class CreateMigrationsTableIfNotExists : Command
    {
        public CreateMigrationsTableIfNotExists(string connectionString)
            : base(connectionString)
        {
            Sql = @"
                IF NOT EXISTS (SELECT 1
                               FROM INFORMATION_SCHEMA.TABLES
                               WHERE TABLE_SCHEMA = 'dbo'
                               AND TABLE_NAME = 'Migrations')
                BEGIN
                    CREATE TABLE [dbo].[Migrations] (
                        [Version]       INT NOT NULL,
                        [AppliedOn]     DATETIME NOT NULL,
                        [Name]          VARCHAR(1024) NOT NULL
                    );
                END
            ";
        }
    }
}
