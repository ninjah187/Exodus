using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.SqlServer.Queries
{
    class GetAppliedMigrationVersions : Query<int[]>
    {
        public GetAppliedMigrationVersions(string connectionString)
            : base(connectionString)
        {
            Sql = "SELECT Version FROM Migrations";
        }

        protected override async Task<int[]> ExecuteQuery(SqlCommand command)
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                var results = new List<int>();
                while (await reader.ReadAsync())
                {
                    results.Add(reader.GetInt32(0));
                }
                return results.ToArray();
            }
        }
    }
}
