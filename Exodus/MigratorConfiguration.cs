using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Exodus
{
    public class MigratorConfiguration
    {
        public string DatabaseConnectionString { get; }
        public string ServerConnectionString { get; }
        public string DatabaseName { get; }

        public MigratorConfiguration(string databaseConnectionString)
        {
            DatabaseConnectionString = databaseConnectionString;

            var connectionStringBuilder = new SqlConnectionStringBuilder(DatabaseConnectionString);
            DatabaseName = connectionStringBuilder.InitialCatalog;

            connectionStringBuilder.InitialCatalog = "";
            ServerConnectionString = connectionStringBuilder.ToString();
        }
    }
}
