﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.SqlServer.Commands
{
    class CreateMigrationsTableIfNotExists : SqlServerCommand
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
                        [Name]          VARCHAR(1024) NOT NULL);
                END
            ";
        }
    }
}
