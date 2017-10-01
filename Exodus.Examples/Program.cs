using System;
using System.Threading.Tasks;

namespace Exodus.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new MigratorConfiguration(
                "Server=(localDb)\\MSSQLLocalDB;Database=exodus.dev;Trusted_Connection=True;");
            var migrator = new Migrator(configuration);
            await migrator
                .DropCreateDatabase()
                .LogToConsole()
                .Migrate();
            Console.ReadKey();
        }
    }
}
