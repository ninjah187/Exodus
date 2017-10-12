using System;
using System.Threading.Tasks;

namespace Exodus.SqlServer.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var migrator = new SqlServerMigrator(
                "Server=(localDb)\\MSSQLLocalDB;Database=exodus.dev;Trusted_Connection=True;");
            await migrator
                .DropCreateDatabase()
                .FromAssembly("Exodus.SqlServer.Examples")
                .LogToConsole()
                .MigrateAsync();
            Console.ReadKey();
        }
    }
}
