using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
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
                .AddQuery();
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseOcelot(configuration =>
            //{
            //    configuration.PreAuthenticationMiddleware = (ctx, next) =>
            //    {
            //        ctx.Response.StatusCode = 401;
            //        return ctx.Response.WriteAsync("Nobody could access");
            //    };
            //});

            app.UseOcelot((ocelotBuilder, ocelotConfiguration) =>
            {
                // this sets up the downstream context and gets the config
                app.UseDownstreamContextMiddleware();
                // This is registered to catch any global exceptions that are not handled
                // It also sets the Request Id if anything is set globally
                ocelotBuilder.UseExceptionHandlerMiddleware();
                // This is registered first so it can catch any errors and issue an appropriate response
                ocelotBuilder.UseResponderMiddleware();
                ocelotBuilder.UseDownstreamRouteFinderMiddleware();
                ocelotBuilder.UseMultiplexingMiddleware();
                ocelotBuilder.UseDownstreamRequestInitialiser();
                ocelotBuilder.UseRequestIdMiddleware();

                ocelotBuilder.UseMiddleware<UrlBasedAuthenticationMiddleware>();
                ocelotBuilder.UseLoadBalancingMiddleware();
                ocelotBuilder.UseDownstreamUrlCreatorMiddleware();
                ocelotBuilder.UseHttpRequesterMiddleware();
            }).Wait();
        }
    }
}
