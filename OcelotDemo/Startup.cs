using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Claims.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamPathManipulation.Middleware;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Ocelot.QueryStrings.Middleware;
using Ocelot.RateLimit.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Security.Middleware;
using OcelotDemo.OcelotMiddleware;
using WeihanLi.Web.Authentication;
using WeihanLi.Web.Authentication.QueryAuthentication;

namespace OcelotDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(QueryAuthenticationDefaults.AuthenticationSchema)
                .AddCustomJwt(
                    JwtBearerDefaults.AuthenticationScheme,
                    Configuration.GetSection("AuthenticationBearerOptions")
                )
                .AddQuery()
                //.AddJwtBearer(x =>
                //{
                //    x.RequireHttpsMetadata = false;
                //    x.Authority = "";
                //    x.TokenValidationParameters = new TokenValidationParameters()
                //    {
                //        ValidateAudience = false,
                //        ClockSkew = TimeSpan.Zero,
                //    };
                //})
                ;
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseOcelot(configuration =>
            //{
            //    configuration.PreAuthenticationMiddleware = (ctx, next) =>
            //    {
            //        ctx.Items.SetError(new UnauthorizedError("No permission"));
            //        return Task.CompletedTask;
            //    };
            //});
            // https://github.com/ThreeMammals/Ocelot/blob/17.0.0/src/Ocelot/Middleware/OcelotPipelineExtensions.cs
            app.UseOcelot((ocelotBuilder, pipelineConfiguration) =>
            {
                // this sets up the downstream context and gets the config
                app.UseDownstreamContextMiddleware();
                // This is registered to catch any global exceptions that are not handled
                // It also sets the Request Id if anything is set globally
                ocelotBuilder.UseExceptionHandlerMiddleware();

                // This is registered first so it can catch any errors and issue an appropriate response
                //ocelotBuilder.UseResponderMiddleware();
                ocelotBuilder.UseMiddleware<CustomResponseMiddleware>();

                ocelotBuilder.UseDownstreamRouteFinderMiddleware();
                ocelotBuilder.UseMultiplexingMiddleware();

                // This security module, IP whitelist blacklist, extended security mechanism
                ocelotBuilder.UseSecurityMiddleware();

                ocelotBuilder.UseHttpHeadersTransformationMiddleware();

                ocelotBuilder.UseDownstreamRequestInitialiser();

                // We check whether the request is ratelimit, and if there is no continue processing
                ocelotBuilder.UseRateLimiting();

                ocelotBuilder.UseRequestIdMiddleware();

                // ocelotBuilder.Use((ctx, next) =>
                // {
                //     ctx.Items.SetError(new UnauthorizedError("No permission"));
                //     return Task.CompletedTask;
                // });

                ocelotBuilder.UseMiddleware<UrlBasedAuthenticationMiddleware>();

                // The next thing we do is look at any claims transforms in case this is important for authorization
                ocelotBuilder.UseClaimsToClaimsMiddleware();

                // Now we can run the claims to headers transformation middleware
                ocelotBuilder.UseClaimsToHeadersMiddleware();

                // Allow the user to implement their own query string manipulation logic
                ocelotBuilder.UseIfNotNull(pipelineConfiguration.PreQueryStringBuilderMiddleware);

                // Now we can run any claims to query string transformation middleware
                ocelotBuilder.UseClaimsToQueryStringMiddleware();

                ocelotBuilder.UseClaimsToDownstreamPathMiddleware();

                ocelotBuilder.UseLoadBalancingMiddleware();
                ocelotBuilder.UseDownstreamUrlCreatorMiddleware();
                ocelotBuilder.UseHttpRequesterMiddleware();
            }).Wait();
        }
    }

    public static class Extensions
    {
        public static void UseIfNotNull(this IApplicationBuilder builder,
            Func<HttpContext, Func<Task>, Task> middleware)
        {
            if (middleware != null)
            {
                builder.Use(middleware);
            }
        }
    }
}
