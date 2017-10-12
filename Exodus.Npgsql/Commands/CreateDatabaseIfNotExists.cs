using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Exodus.Npgsql.Queries;
using System.Threading.Tasks;

namespace Exodus.Npgsql.Commands
{
    class CreateDatabaseIfNotExists
    {
        readonly string _serverConnectionString;
        readonly string _databaseName;

        public CreateDatabaseIfNotExists(string serverConnectionString, string databaseName)
        {
            _serverConnectionString = serverConnectionString;
            _databaseName = databaseName;
        }

        public async Task Execute()
        {
            var checkIfDatabaseExists = new CheckIfDatabaseExists(_serverConnectionString, _databaseName);
            if (await checkIfDatabaseExists.Execute())
            {
                return;
            }
            var createDatabase = new CreateDatabase(_serverConnectionString, _databaseName);
            await createDatabase.Execute();
        }
    }
}
