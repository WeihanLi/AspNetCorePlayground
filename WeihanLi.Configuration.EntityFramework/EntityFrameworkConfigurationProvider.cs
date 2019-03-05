using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.EntityFramework
{
    internal class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        private readonly DbContextOptions<ConfigurationsDbContext> _dbContextOptions;

        public EntityFrameworkConfigurationProvider(DbContextOptions<ConfigurationsDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public override void Load()
        {
            using (var dbContext = new ConfigurationsDbContext(_dbContextOptions))
            {
                var configurations = dbContext.Configurations.AsNoTracking()
                    .ToArray();
                if (configurations.Length == 0)
                    return;

                foreach (var configuration in configurations)
                {
                    Data[configuration.Key] = configuration.Value;
                }
            }
        }
    }
}
