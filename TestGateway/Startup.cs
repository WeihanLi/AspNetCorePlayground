using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authentication.Middleware;
using Ocelot.Authorisation.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using TestGateway.OcelotMiddlewares;
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
            services.AddMvc();

            var tokenOptions = new JwtTokenOptions();
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.SecretKey));
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // The signing key must match!
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = signingKey,
                            // Validate the JWT Issuer (iss) claim
                            ValidateIssuer = true,
                            ValidIssuer = tokenOptions.Issuer,
                            // Validate the JWT Audience (aud) claim
                            ValidateAudience = true,
                            ValidAudience = tokenOptions.Audience,
                            // Validate the token expiry
                            ValidateLifetime = true,
                            // If you want to allow a certain amount of clock drift, set that here:
                            ClockSkew = System.TimeSpan.Zero
                        };
                    });

            services.AddOcelot()
                //.StoreConfigurationInEntityFramework(options => options.UseSqlServer(Configuration.GetConnectionString("OcelotConfigurations")))
                .StoreConfigurationInRedis()
                ;

            services.AddRedisConfig(options =>
            {
                options.RedisServers = new[]
                {
                    new RedisServerConfiguration("127.0.0.1", 6379),
                };
                options.DefaultDatabase = 2;
                options.CachePrefix = "AspNetCorePlayground";
            });
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
                    // WARN: this api should be protected with permissions
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

            app.UseOcelotWhenRouteMatch((ocelotBuilder, pipelineConfiguration) =>
                {
                    // This is registered to catch any global exceptions that are not handled
                    // It also sets the Request Id if anything is set globally
                    ocelotBuilder.UseExceptionHandlerMiddleware();
                    // This is registered first so it can catch any errors and issue an appropriate response
                    ocelotBuilder.UseResponderMiddleware();
                    ocelotBuilder.UseDownstreamRouteFinderMiddleware();
                    ocelotBuilder.UseDownstreamRequestInitialiser();
                    ocelotBuilder.UseRequestIdMiddleware();
                    ocelotBuilder.UseMiddleware<ClaimsToHeadersMiddleware>();
                    ocelotBuilder.UseAuthorisationMiddleware();
                    ocelotBuilder.UseAuthenticationMiddleware();
                    ocelotBuilder.UseLoadBalancingMiddleware();
                    ocelotBuilder.UseDownstreamUrlCreatorMiddleware();
                    ocelotBuilder.UseOutputCacheMiddleware();
                    ocelotBuilder.UseMiddleware<HttpRequesterMiddleware>();
                    // cors headers
                    ocelotBuilder.UseMiddleware<CorsMiddleware>();
                });

            app.UseMvc();
        }
    }
}
