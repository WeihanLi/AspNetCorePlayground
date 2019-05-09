using System;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;

namespace WeihanLi.Ocelot.ConfigurationProvider.Redis
{
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// StoreConfigurationInEntityFramework
        /// </summary>
        /// <param name="builder">the OcelotBuilder</param>
        /// <returns>the OcelotBuilder</returns>
        public static IOcelotBuilder StoreConfigurationInRedis(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton<IFileConfigurationRepository, RedisFileConfigurationRepository>();
            return builder;
        }

        /// <summary>
        /// StoreConfigurationInEntityFramework
        /// </summary>
        /// <param name="builder">the OcelotBuilder</param>
        /// <param name="optionsAction">Ocelot DbContext optionsAction</param>
        /// <returns>the OcelotBuilder</returns>
        public static IOcelotBuilder StoreConfigurationInRedis(this IOcelotBuilder builder, Action<RedisFileConfigurationOptions> optionsAction)
        {
            if (optionsAction != null)
            {
                builder.Services.Configure(optionsAction);
            }
            builder.StoreConfigurationInRedis();
            return builder;
        }
    }
}
