using Exodus.Communication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Exodus.Commands
{
    class DropDatabaseIfExists : Command
    {
        readonly string _databaseName;

        public DropDatabaseIfExists(string connectionString, string databaseName)
            : base(connectionString)
        {
            _databaseName = databaseName;
            Sql = $@"
                IF EXISTS (SELECT * FROM sys.databases WHERE name=@databaseName)
                BEGIN
                    --- kill all connections to the database before drop ---
                    DECLARE @sql VARCHAR(MAX)

                    SELECT  @sql = COALESCE(@sql, '') + 'Kill ' + CONVERT(varchar, SPId) + ';'
                    FROM    MASTER..SysProcesses
                    WHERE   DBId = db_id(@databaseName) AND SPId <> @@SPId

                    IF (@sql IS NOT NULL)
                    BEGIN
                        EXEC(@sql)
                    END
                    --------------------------------------------------------

                    DROP DATABASE [{databaseName}];
                END
            ";
        }

        protected override void AddParameters(SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter("databaseName", _databaseName));
        }
    }
}
