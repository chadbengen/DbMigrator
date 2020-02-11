using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DbMigrator
{
    public interface IMigrateAndSeed
    {
        Task Migrate(IServiceCollection services);
        Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot);
    }
}
