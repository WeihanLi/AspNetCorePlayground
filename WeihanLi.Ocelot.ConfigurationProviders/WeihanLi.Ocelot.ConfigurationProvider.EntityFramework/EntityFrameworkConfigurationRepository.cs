using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using WeihanLi.Ocelot.ConfigurationProvider.EntityFramework.Models;

namespace WeihanLi.Ocelot.ConfigurationProvider.EntityFramework
{
    internal class EntityFrameworkConfigurationRepository : IFileConfigurationRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EFConfigurationRepositoryOptions _configurationRepositoryOptions;

        public EntityFrameworkConfigurationRepository(IServiceProvider serviceProvider, IOptions<EFConfigurationRepositoryOptions> options)
        {
            _serviceProvider = serviceProvider;
            _configurationRepositoryOptions = options.Value;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            Expression<Func<OcelotConfiguration, bool>> expression = c => c.IsEnabled;
            if (_configurationRepositoryOptions.OcelotConfigurationId > 0)
            {
                expression = c => c.IsEnabled && c.Id == _configurationRepositoryOptions.OcelotConfigurationId;
            }

            OcelotConfiguration configuration;
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<OcelotDbContext>())
                {
                    configuration = await context.OcelotConfigurations
                        .OrderByDescending(_ => _.Id)
                        .FirstOrDefaultAsync(expression);

                    if (configuration == null)
                        return new OkResponse<FileConfiguration>(new FileConfiguration());

                    configuration.GlobalConfiguration =
                        await context.GlobalConfigurations.FirstOrDefaultAsync(_ => _.ConfigurationId == configuration.Id) ?? new GlobalConfiguration();
                    configuration.ReRoutes =
                        await context.ReRoutes.Where(_ => _.ConfigurationId == configuration.Id).ToListAsync();
                }
            }

            var fileConfiguration = new FileConfiguration()
            {
                GlobalConfiguration = new FileGlobalConfiguration()
                {
                    BaseUrl = configuration.GlobalConfiguration.BaseUrl,
                    DownstreamScheme = configuration.GlobalConfiguration.DownstreamScheme,
                    RequestIdKey = configuration.GlobalConfiguration.RequestIdKey,
                    HttpHandlerOptions = new FileHttpHandlerOptions()
                    {
                        AllowAutoRedirect = false,
                        UseCookieContainer = false,
                        UseProxy = false,
                        UseTracing = false,
                    }
                },
            };

            foreach (var reRoute in configuration.ReRoutes)
            {
                var fileReRoute = new FileReRoute
                {
                    DownstreamScheme = reRoute.DownstreamScheme,
                    DownstreamPathTemplate = reRoute.DownstreamPathTemplate,
                    DownstreamHostAndPorts = JsonConvert.DeserializeObject<List<FileHostAndPort>>(reRoute.DownstreamHostAndPorts),
                    UpstreamHttpMethod = JsonConvert.DeserializeObject<List<string>>(reRoute.UpstreamHttpMethod),
                    UpstreamPathTemplate = reRoute.UpstreamPathTemplate,
                    UpstreamHost = reRoute.UpstreamHost,
                    RequestIdKey = reRoute.RequestIdKey,
                    ReRouteIsCaseSensitive = reRoute.ReRouteIsCaseSensitive,
                };
                if (!string.IsNullOrEmpty(reRoute.AuthenticationOption))
                {
                    fileReRoute.AuthenticationOptions = JsonConvert.DeserializeObject<FileAuthenticationOptions>(reRoute.AuthenticationOption);
                }
                if (!string.IsNullOrEmpty(reRoute.CacheOption))
                {
                    fileReRoute.FileCacheOptions = JsonConvert.DeserializeObject<FileCacheOptions>(reRoute.CacheOption);
                }
                if (!string.IsNullOrEmpty(reRoute.HttpHandlerOption))
                {
                    fileReRoute.HttpHandlerOptions = JsonConvert.DeserializeObject<FileHttpHandlerOptions>(reRoute.HttpHandlerOption);
                }
                if (!string.IsNullOrEmpty(reRoute.RateLimitOption))
                {
                    fileReRoute.RateLimitOptions = JsonConvert.DeserializeObject<FileRateLimitRule>(reRoute.RateLimitOption);
                }
                if (!string.IsNullOrEmpty(reRoute.AddHeadersToRequest))
                {
                    fileReRoute.AddHeadersToRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(reRoute.AddHeadersToRequest);
                }
                if (!string.IsNullOrEmpty(reRoute.AddClaimsToRequest))
                {
                    fileReRoute.AddClaimsToRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(reRoute.AddClaimsToRequest);
                }
                if (!string.IsNullOrEmpty(reRoute.AddQueriesToRequest))
                {
                    fileReRoute.AddQueriesToRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(reRoute.AddQueriesToRequest);
                }
                //
                fileConfiguration.ReRoutes.Add(fileReRoute);
            }
            return new OkResponse<FileConfiguration>(fileConfiguration);
        }

        public Task<Response> Set(FileConfiguration fileConfiguration)
        {
            return Task.FromResult((Response)new OkResponse());
        }
    }
}
