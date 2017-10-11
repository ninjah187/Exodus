using System;
using System.Threading.Tasks;

namespace Exodus.Npgsql.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var migrator = new NpgsqlMigrator(
                "User ID=postgres;" +
                "Password=postgres;" +
                "Host=localhost;" +
                "Port=5433;" +
                "Database=exodus.dev;" +
                "Pooling=true;");
            await migrator
                .DropCreateDatabase()
                .FromAssembly("Exodus.Npgsql.Migrations")
                .LogToConsole()
                .MigrateAsync();
            Console.ReadKey();
        }
    }
}
