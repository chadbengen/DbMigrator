using EFMigrateAndSeed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{

    public class MyDbContextMigrator : MigrateAndSeedBase
    {
        public MyDbContextMigrator(ILogger<MyDbContextMigrator> logger) : base(logger)
        {
        }

        public override Task Migrate(IServiceCollection services)
        {
            return base.MigrateDbContext<MyDbContext>(services.BuildServiceProvider());
        }

        public override async Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            var serviceProvider = services.BuildServiceProvider();
            
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var seedData = scope.ServiceProvider.GetRequiredService<SeedPersons>();
               
                _logger.LogDebug("Retrieved seed data for {PersonCount} Person(s).", seedData.Persons.Count, seedData);

                using (var context = scope.ServiceProvider.GetRequiredService<MyDbContext>())
                {
                    foreach (var person in seedData.Persons)
                    {
                        if (!context.Persons.Any(a => a.Name == person.Name))
                        {
                            person.Date = DateTime.UtcNow;
                            context.Persons.Add(person);
                            await context.SaveChangesAsync();
                            _logger.LogDebug("Person {PersonName} was added.", person.Name, person);
                        }
                        else
                        {
                            _logger.LogDebug("Person {PersonName} already exists!", person.Name, person);
                        }
                    }
                }
            }

            _logger.LogInformation("Seeded {SourceContext}.");
        }

        public static void Register(IServiceCollection services, IConfigurationRoot configuration)
        {
            var seed = new SeedPersons();
            configuration.Bind(seed);
            services.AddSingleton(seed);

            services.AddScoped<IMigrateAndSeed, MyDbContextMigrator>();
            
            services.AddDbContext<MyDbContext>();
        }
    }
}
