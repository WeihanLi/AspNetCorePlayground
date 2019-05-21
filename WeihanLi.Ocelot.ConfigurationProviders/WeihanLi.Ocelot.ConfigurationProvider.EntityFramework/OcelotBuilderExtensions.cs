using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.Repository;
using WeihanLi.Ocelot.ConfigurationProvider.EntityFramework;

// ReSharper disable once CheckNamespace
namespace Ocelot.DependencyInjection
{
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// Store OcelotConfiguration in EntityFramework
        /// </summary>
        /// <param name="builder">the OcelotBuilder</param>
        /// <param name="optionsAction">Ocelot DbContext optionsAction</param>
        /// <returns>the OcelotBuilder</returns>
        public static IOcelotBuilder StoreConfigurationInEntityFramework(this IOcelotBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction)
        {
            builder.Services.AddDbContextPool<OcelotDbContext>(optionsAction, poolSize: 100); // dbContextPool size tip https://www.cnblogs.com/dudu/p/10398225.html
            builder.Services.AddSingleton<IFileConfigurationRepository, EntityFrameworkConfigurationRepository>();

            return builder;
        }

        /// <summary>
        /// Store OcelotConfiguration in EntityFramework
        /// </summary>
        /// <param name="builder">the OcelotBuilder</param>
        /// <param name="optionsAction">Ocelot DbContext optionsAction</param>
        /// <param name="repoOptionsAction">repoOptionsAction</param>
        /// <returns>the OcelotBuilder</returns>
        public static IOcelotBuilder StoreConfigurationInEntityFramework(this IOcelotBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction,
            Action<EFConfigurationRepositoryOptions> repoOptionsAction)
        {
            if (null != repoOptionsAction)
            {
                builder.Services.Configure(repoOptionsAction);
            }

            builder.StoreConfigurationInEntityFramework(optionsAction);
            return builder;
        }
    }
}
