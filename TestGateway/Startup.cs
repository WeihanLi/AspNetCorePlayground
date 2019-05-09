using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Ocelot.Cache.Middleware;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using WeihanLi.Extensions;
using WeihanLi.Ocelot.ConfigurationProvider.Redis;
using WeihanLi.Redis;

namespace TestGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRedisConfig(options =>
            {
                options.RedisServers = new[]
                {
                    new RedisServerConfiguration("127.0.0.1", 6379),
                };
                options.DefaultDatabase = 2;
                options.CachePrefix = "AspNetCorePlayground";
            });
            services.AddOcelot()
                .StoreConfigurationInRedis();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ICacheClient cacheClient)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            TestGetCache(cacheClient);

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

            app.UseOcelot((ocelotBuilder, pipelineConfiguration) =>
                {
                    // This is registered to catch any global exceptions that are not handled
                    // It also sets the Request Id if anything is set globally
                    ocelotBuilder.UseExceptionHandlerMiddleware();
                    // Allow the user to respond with absolutely anything they want.
                    if (pipelineConfiguration != null)
                    {
                        ocelotBuilder.Use(pipelineConfiguration.PreErrorResponderMiddleware);
                    }
                    // This is registered first so it can catch any errors and issue an appropriate response
                    ocelotBuilder.UseResponderMiddleware();
                    ocelotBuilder.UseDownstreamRouteFinderMiddleware();
                    ocelotBuilder.UseDownstreamRequestInitialiser();
                    ocelotBuilder.UseRequestIdMiddleware();
                    ocelotBuilder.UseMiddleware<ClaimsToHeadersMiddleware>();
                    ocelotBuilder.UseLoadBalancingMiddleware();
                    ocelotBuilder.UseDownstreamUrlCreatorMiddleware();
                    ocelotBuilder.UseOutputCacheMiddleware();
                    ocelotBuilder.UseMiddleware<HttpRequesterMiddleware>();
                    // cors headers
                    ocelotBuilder.Use(async (context, next) =>
                    {
                        var allowedOrigins = Configuration.GetAppSetting("AllowedOrigins");
                        context.DownstreamResponse.Headers.Add(new Header(HeaderNames.AccessControlAllowOrigin, allowedOrigins.SplitArray<string>()));
                        context.DownstreamResponse.Headers.Add(new Header(HeaderNames.AccessControlAllowHeaders, new[] { "*" }));
                        context.DownstreamResponse.Headers.Add(new Header(HeaderNames.AccessControlRequestMethod, new[] { "*" }));
                        await next();
                    });
                })
                .Wait();
        }

        private void TestGetCache(ICacheClient cacheClient)
        {
            var cache = cacheClient.Get("OcelotConfigurations");
            cache = cacheClient.GetAsync("OcelotConfigurations").Result;
        }
    }
}
