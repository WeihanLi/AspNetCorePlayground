using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeihanLi.Redis;

namespace WeihanLi.Configuration.Redis
{
    public class RedisConfigurationSource : IConfigurationSource
    {
        public IServiceCollection Services { get; set; }
        private readonly IServiceProvider _serviceProvider;

        public string RedisConfigurationKey { get; set; }

        public RedisConfigurationSource()
        {
        }

        public RedisConfigurationSource(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var option = _serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
            var hashClient = _serviceProvider.GetRequiredService<IHashClient>();
            return new RedisConfigurationProvider(hashClient, option);
        }
    }
}
