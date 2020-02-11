using EFMigrateAndSeed;
using System;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //// To close the console window when debugging is finished:
            //// Debug > Options > General > Automatically close the console when debugging stops
            ////https://developercommunity.visualstudio.com/content/problem/321978/how-to-remove-exited-with-code-0-from-cmd-console.html

            await MigratorBuilder
                .Create()
                .SetBasePath(@"../../../")
                .AddJsonFile("seed.json", false)
                .Register(MyDbContextMigrator.Register)
                .Execute<Program>();
     
            Console.WriteLine("\npress any key to exit the process...");
            Console.ReadKey();
        }
    }
}
