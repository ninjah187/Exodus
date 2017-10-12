using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Threading.Tasks;
using Exodus.Npgsql.Queries;

namespace Exodus.Npgsql.Commands
{
    class DropDatabaseIfExists
    {
        readonly string _serverConnectionString;
        readonly string _databaseName;

        public DropDatabaseIfExists(
            string serverConnectionString,
            string databaseName)
        {
            _serverConnectionString = serverConnectionString;
            _databaseName = databaseName;
        }

        public async Task Execute()
        {
            var checkIfDatabaseExists = new CheckIfDatabaseExists(_serverConnectionString, _databaseName);
            if (!await checkIfDatabaseExists.Execute())
            {
                return;
            }
            var killAllConnections = new KillAllConnections(_serverConnectionString, _databaseName);
            var dropDatabase = new DropDatabase(_serverConnectionString, _databaseName);
            await killAllConnections.Execute();
            await dropDatabase.Execute();
        }
    }
}
