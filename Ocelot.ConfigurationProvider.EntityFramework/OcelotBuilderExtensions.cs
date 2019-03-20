using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.Repository;
using Ocelot.ConfigurationProvider.EntityFramework;

// ReSharper disable once CheckNamespace
namespace Ocelot.DependencyInjection
{
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// StoreConfigurationInEntityFramework
        /// </summary>
        /// <param name="builder">the OcelotBuilder</param>
        /// <param name="optionsAction">Ocelot DbContext optionsAction</param>
        /// <param name="configurationId">configurationId</param>
        /// <returns>the OcelotBuilder</returns>
        public static IOcelotBuilder StoreConfigurationInEntityFramework(this IOcelotBuilder builder, Action<DbContextOptionsBuilder> optionsAction, int configurationId = -1)
        {
            EntityFrameworkConfigurationRepository.SpecificConfigurationId = configurationId;

            builder.Services.AddDbContextPool<OcelotDbContext>(optionsAction, poolSize: 64); // dbContextPool size tip https://www.cnblogs.com/dudu/p/10398225.html
            builder.Services.AddScoped<IFileConfigurationRepository, EntityFrameworkConfigurationRepository>();

            return builder;
        }
    }
}
