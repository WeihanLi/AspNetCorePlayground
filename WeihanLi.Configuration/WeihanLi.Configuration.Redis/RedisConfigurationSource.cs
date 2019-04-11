using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeihanLi.Redis;

namespace WeihanLi.Configuration.Redis
{
    public class RedisConfigurationSource : IConfigurationSource
    {
        public IServiceCollection Services { get; set; }

        public string RedisConfigurationKey { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var serviceProvider = Services.BuildServiceProvider();
            var option = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
            var hashClient = serviceProvider.GetRequiredService<IHashClient>();
            return new RedisConfigurationProvider(hashClient, option);
        }
    }
}
