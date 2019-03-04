using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Configuration.EntityFramework
{
    public class EntityFrameworkConfigurationSource : IConfigurationSource
    {
        public IServiceCollection Services { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var serviceProvider = Services?.BuildServiceProvider();
            return new EntityFrameworkConfigurationProvider(serviceProvider.GetService<ConfigurationsDbContext>());
        }
    }
}
