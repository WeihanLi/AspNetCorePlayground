using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.ConfigurationProvider.EntityFramework.Models;
using Ocelot.Responses;

namespace Ocelot.ConfigurationProvider.EntityFramework
{
    internal class EntityFrameworkConfigurationRepository : IFileConfigurationRepository
    {
        private readonly OcelotDbContext _context;
        internal static int SpecificConfigurationId = -1;

        public EntityFrameworkConfigurationRepository(OcelotDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            Expression<Func<OcelotConfiguration, bool>> expression = c => c.IsEnabled;
            if (SpecificConfigurationId > 0)
            {
                expression = c => c.IsEnabled && c.Id == SpecificConfigurationId;
            }
            var configuration = await _context.OcelotConfigurations
                .OrderByDescending(_ => _.Id)
                .FirstOrDefaultAsync(expression);

            if (configuration == null)
                return new OkResponse<FileConfiguration>(new FileConfiguration());

            configuration.GlobalConfiguration =
                await _context.GlobalConfigurations.FirstOrDefaultAsync(_ => _.ConfigurationId == configuration.Id) ?? new GlobalConfiguration();
            configuration.ReRoutes =
                await _context.ReRoutes.Where(_ => _.ConfigurationId == configuration.Id).ToListAsync();

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
