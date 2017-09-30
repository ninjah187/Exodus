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
            var configuration = new MigratorConfiguration(
                "Server=(localDb)\\MSSQLLocalDB;Database=exodus.dev;Trusted_Connection=True;");
            var migrator = new Migrator(configuration);
            await migrator
                .DropCreateDatabase()
                .Migrate();
            Console.ReadKey();
        }
    }
}
