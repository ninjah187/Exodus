using System;
using System.Threading.Tasks;

namespace Exodus.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var migrator = new Migrator(
                "Server=(localDb)\\MSSQLLocalDB;Database=exodus.dev;Trusted_Connection=True;");
            await migrator
                .DropCreateDatabase()
                .FromAssembly("Exodus.Migrations")
                .LogToConsole()
                .MigrateAsync();
            Console.ReadKey();
        }
    }
}
