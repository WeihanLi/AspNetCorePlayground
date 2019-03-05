using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.EntityFramework
{
    public static class RedisConfigurationExtension
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from EntityFramework.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="action">Configures the configurationsDbContext source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEntityFramework(this IConfigurationBuilder builder, Action<DbContextOptionsBuilder<ConfigurationsDbContext>> action)
            => builder.Add(new EntityFrameworkConfigurationSource(action));
    }
}
