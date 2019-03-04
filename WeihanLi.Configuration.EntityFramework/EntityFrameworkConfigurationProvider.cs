using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.EntityFramework
{
    internal class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        private readonly ConfigurationsDbContext _dbContext;

        public EntityFrameworkConfigurationProvider(ConfigurationsDbContext dbContext) => _dbContext = dbContext;

        public override void Load()
        {
            var configurations = _dbContext.Configurations.AsNoTracking()
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
