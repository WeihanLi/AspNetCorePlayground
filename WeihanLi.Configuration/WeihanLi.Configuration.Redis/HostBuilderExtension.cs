using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeihanLi.Redis;

namespace WeihanLi.Configuration.Redis
{
    public static class WebHostBuilderExtension
    {
        public static IWebHostBuilder UseRedisConfiguration(this IWebHostBuilder webHostBuilder,
            Action<IConfiguration, RedisConfigurationOptions> redisOptionsAction,
            Action<RedisConfigurationSource> configAction = null)
        {
            webHostBuilder.ConfigureServices((builderContext, services) =>
            {
                if (configAction == null)
                {
                    configAction = src => { };
                }
                services.AddRedisConfig(builderContext.Configuration, redisOptionsAction);
                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddConfiguration(builderContext.Configuration);
                configurationBuilder.AddRedis(configAction);
                var configuration = configurationBuilder.Build();
                services.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));
            });

            return webHostBuilder;
        }
    }
}
