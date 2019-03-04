using System;
using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.EntityFramework
{
    public static class RedisConfigurationExtension
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from environment variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEntityFramework(this IConfigurationBuilder builder, Action<EntityFrameworkConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
