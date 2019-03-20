using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using WeihanLi.Ocelot.ConfigurationProvider.Redis;
using WeihanLi.Redis;

namespace TestGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRedisConfig(options =>
            {
                options.RedisServers = new RedisServerConfiguration[]
                {
                    new RedisServerConfiguration("127.0.0.1", 6379),
                };
                options.EnableCompress = false; // disable value compress
                options.DefaultDatabase = 2;
                options.CachePrefix = "AspNetCorePlayground";
            });
            services.AddOcelot()
                .StoreConfigurationInRedis();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 更新 Ocelot 配置接口

            // PUT /ocelot/admin/configuration 需要 Admin Role

            app.Map(new PathString("/ocelot/admin/configuration"), appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var configurationRepo =
                        context.RequestServices.GetRequiredService<IFileConfigurationRepository>();
                    var ocelotConfiguration = await configurationRepo.Get();
                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OcelotConfiguration");
                    if (!ocelotConfiguration.IsError)
                    {
                        var internalConfigurationRepo = context.RequestServices.GetRequiredService<IInternalConfigurationRepository>();
                        var internalConfigurationCreator =
                            context.RequestServices.GetRequiredService<IInternalConfigurationCreator>();
                        var internalConfiguration = await internalConfigurationCreator.Create(ocelotConfiguration.Data);
                        if (!internalConfiguration.IsError)
                        {
                            internalConfigurationRepo.AddOrReplace(internalConfiguration.Data);
                            context.Response.StatusCode = 200;
                            return;
                        }
                        else
                        {
                            logger.LogError($"update ocelot configuration error, error in create ocelot internal configuration, error messages:{string.Join(", ", ocelotConfiguration.Errors)}");
                        }
                    }
                    else
                    {
                        logger.LogError($"update ocelot configuration error, error in get ocelot configuration from configurationRepo, error messages:{string.Join(", ", ocelotConfiguration.Errors)}");
                    }
                    context.Response.StatusCode = 500;
                });
            });

            #endregion 更新 Ocelot 配置接口

            app.UseOcelot().Wait();
        }
    }
}
