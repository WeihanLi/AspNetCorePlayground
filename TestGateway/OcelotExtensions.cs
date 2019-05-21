using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.Configuration;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Setter;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Responses;

namespace TestGateway
{
    public static class OcelotExtensions
    {
        public static IApplicationBuilder UseOcelotWhenRouteMatch(this IApplicationBuilder app,
            Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction)
            => UseOcelotWhenRouteMatch(app, builderAction, new OcelotPipelineConfiguration());

        public static IApplicationBuilder UseOcelotWhenRouteMatch(this IApplicationBuilder app,
            Action<OcelotPipelineConfiguration> pipelineConfigurationAction,
            Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction)
        {
            var pipelineConfiguration = new OcelotPipelineConfiguration();
            pipelineConfigurationAction?.Invoke(pipelineConfiguration);
            return UseOcelotWhenRouteMatch(app, builderAction, pipelineConfiguration);
        }

        public static IApplicationBuilder UseOcelotWhenRouteMatch(this IApplicationBuilder app, Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction, OcelotPipelineConfiguration configuration)
        {
            app.MapWhen(context =>
            {
                var internalConfigurationResponse =
                    context.RequestServices.GetRequiredService<IInternalConfigurationRepository>().Get();
                if (internalConfigurationResponse.IsError || internalConfigurationResponse.Data.ReRoutes.Count == 0)
                {
                    return false;
                }

                var internalConfiguration = internalConfigurationResponse.Data;
                var downstreamRouteFinder = context.RequestServices
                    .GetRequiredService<IDownstreamRouteProviderFactory>()
                    .Get(internalConfiguration);
                var response = downstreamRouteFinder.Get(context.Request.Path, context.Request.QueryString.ToString(),
                    context.Request.Method, internalConfiguration, context.Request.Host.ToString());
                return !response.IsError
                       && !string.IsNullOrEmpty(response.Data?.ReRoute?.DownstreamReRoute?.FirstOrDefault()
                           ?.DownstreamScheme);
            }, appBuilder => appBuilder.UseOcelot(builderAction, configuration).Wait());

            return app;
        }

        public static Task<IApplicationBuilder> UseOcelot(this IApplicationBuilder app,
            Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction)
            => UseOcelot(app, builderAction, new OcelotPipelineConfiguration());

        public static Task<IApplicationBuilder> UseOcelot(this IApplicationBuilder app,
            Action<OcelotPipelineConfiguration> pipelineConfigurationAction,
            Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction)
        {
            var pipelineConfiguration = new OcelotPipelineConfiguration();
            pipelineConfigurationAction?.Invoke(pipelineConfiguration);
            return UseOcelot(app, builderAction, pipelineConfiguration);
        }

        public static async Task<IApplicationBuilder> UseOcelot(this IApplicationBuilder app, Action<IOcelotPipelineBuilder, OcelotPipelineConfiguration> builderAction, OcelotPipelineConfiguration configuration)
        {
            var internalConfiguration = await CreateConfiguration(app); // initConfiguration

            ConfigureDiagnosticListener(app);

            var ocelotPipelineBuilder = new OcelotPipelineBuilder(app.ApplicationServices);
            builderAction?.Invoke(ocelotPipelineBuilder, configuration ?? new OcelotPipelineConfiguration());

            var ocelotDelegate = ocelotPipelineBuilder.Build();
            app.Properties["analysis.NextMiddlewareName"] = "TransitionToOcelotMiddleware";

            app.Use(async (context, task) =>
            {
                var downstreamContext = new DownstreamContext(context);
                await ocelotDelegate.Invoke(downstreamContext);
            });

            return app;
        }

        private static async Task<IInternalConfiguration> CreateConfiguration(IApplicationBuilder builder)
        {
            var fileConfigurationRepository = builder.ApplicationServices.GetService<IFileConfigurationRepository>();
            var fileConfiguration = await fileConfigurationRepository.Get();
            if (fileConfiguration.IsError)
            {
                ThrowToStopOcelotStarting(fileConfiguration);
            }

            // now create the config
            var internalConfigCreator = builder.ApplicationServices.GetService<IInternalConfigurationCreator>();
            var internalConfig = await internalConfigCreator.Create(fileConfiguration.Data);

            // Configuration error, throw error message
            if (internalConfig.IsError)
            {
                ThrowToStopOcelotStarting(internalConfig);
            }

            // now save it in memory
            var internalConfigRepo = builder.ApplicationServices.GetService<IInternalConfigurationRepository>();
            internalConfigRepo.AddOrReplace(internalConfig.Data);

            // if configuration from file system
            if (typeof(DiskFileConfigurationRepository) == fileConfigurationRepository.GetType())
            {
                // earlier user needed to add ocelot files in startup configuration stuff, asp.net will map it to this
                var fileConfig = builder.ApplicationServices.GetService<IOptionsMonitor<FileConfiguration>>();
                fileConfig.OnChange(async (config) =>
                {
                    var newInternalConfig = await internalConfigCreator.Create(config);
                    internalConfigRepo.AddOrReplace(newInternalConfig.Data);
                });
            }

            var adminPath = builder.ApplicationServices.GetService<IAdministrationPath>();

            var configurations = builder.ApplicationServices.GetServices<OcelotMiddlewareConfigurationDelegate>();
            // Todo - this has just been added for consul so far...will there be an ordering problem in the future? Should refactor all config into this pattern?
            foreach (var configuration in configurations)
            {
                await configuration(builder);
            }

            if (AdministrationApiInUse(adminPath))
            {
                //We have to make sure the file config is set for the ocelot.env.json and ocelot.json so that if we pull it from the
                //admin api it works...boy this is getting a spit spags boll.
                var fileConfigSetter = builder.ApplicationServices.GetService<IFileConfigurationSetter>();
                await SetFileConfig(fileConfigSetter, fileConfiguration.Data);
            }

            return GetOcelotConfigAndReturn(internalConfigRepo);
        }

        private static bool AdministrationApiInUse(IAdministrationPath adminPath)
        {
            return adminPath != null;
        }

        private static async Task SetFileConfig(IFileConfigurationSetter fileConfigSetter, FileConfiguration fileConfig)
        {
            var response = await fileConfigSetter.Set(fileConfig);

            if (IsError(response))
            {
                ThrowToStopOcelotStarting(response);
            }
        }

        private static bool IsError(Response response)
        {
            return response == null || response.IsError;
        }

        private static IInternalConfiguration GetOcelotConfigAndReturn(IInternalConfigurationRepository provider)
        {
            var ocelotConfiguration = provider.Get();

            if (ocelotConfiguration?.Data == null || ocelotConfiguration.IsError)
            {
                ThrowToStopOcelotStarting(ocelotConfiguration);
                return null;
            }

            return ocelotConfiguration.Data;
        }

        private static void ThrowToStopOcelotStarting(Response config)
        {
            throw new Exception($"Unable to start Ocelot, errors are: {string.Join(",", config.Errors.Select(x => x.ToString()))}");
        }

        private static void ConfigureDiagnosticListener(IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService<IHostingEnvironment>();
            var listener = builder.ApplicationServices.GetService<OcelotDiagnosticListener>();
            var diagnosticListener = builder.ApplicationServices.GetService<DiagnosticListener>();
            diagnosticListener.SubscribeWithAdapter(listener);
        }
    }
}
