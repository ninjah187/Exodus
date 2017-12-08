using System;
using System.Threading.Tasks;

namespace Exodus.SqlServer.Examples
{
    class Program
    {
        //static async Task Main(string[] args)
        //{
        //    var migrator = new SqlServerMigrator(
        //        "Server=(localDb)\\MSSQLLocalDB;" +
        //        "Database=exodus.dev;" +
        //        "Trusted_Connection=True;");
        //    await migrator
        //        .DropCreateDatabase()
        //        .FromAssembly("dsadsad")
        //        .LogToConsole()
        //        .MigrateAsync();
        //    Console.ReadKey();
        //}

        static void Main(string[] args)
        {
            var migrator = new SqlServerMigrator(
                "Server=(localDb)\\MSSQLLocalDB;" +
                "Database=exodus.dev;" +
                "Trusted_Connection=True;");
            migrator
                .DropCreateDatabase()
                .FromAssembly("Exodus.SqlServer.Examples")
                .LogToConsole()
                .Migrate();
            Console.ReadKey();
        }
    }
}
