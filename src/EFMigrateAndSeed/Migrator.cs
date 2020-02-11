using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using EFMigrateAndSeed.Extensions;
using EFMigrateAndSeed.Dependency;

namespace EFMigrateAndSeed
{
    public class Migrator
    {

        public IConfigurationBuilder ConfigurationBuilder { get; } = new ConfigurationBuilder();
        public IConfigurationRoot ConfigurationRoot { get; protected set; }
        public IServiceCollection Services { get; protected set; }

        public virtual void BuildConfiguration<T>() where T : class
        {
            ConfigurationRoot = ConfigurationBuilder
                .AddUserSecrets<T>(true)
                .Build();
        }

        public virtual void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        public virtual void InitializeServices()
        {
            Services = new ServiceCollection();
            Services.AddLogging(s => s.AddSerilog());
        }

        public virtual async Task Execute()
        {
            var migrators = Services
               .BuildServiceProvider()
               .GetServices<IMigrateAndSeed>()
               .Select(m => new DependencyItem(m.GetType(), m))
               .ToList()
               .SetDependencies()
               .SortByDependencies(m => m.Dependencies)
               .Select(m => m.Instance as IMigrateAndSeed);


            foreach (var migrator in migrators)
            {
                await migrator.Migrate(Services);

                await migrator.Seed(Services, ConfigurationRoot);

                Console.WriteLine("-------------------------------------------");
                Console.WriteLine();
            }
        }
    }
}
