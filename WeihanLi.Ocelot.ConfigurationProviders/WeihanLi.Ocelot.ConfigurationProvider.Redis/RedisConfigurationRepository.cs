using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using WeihanLi.Redis;

namespace WeihanLi.Ocelot.ConfigurationProvider.Redis
{
    internal class RedisFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly ICacheClient _cacheClient;
        private readonly RedisFileConfigurationOptions _redisFileConfigurationOptions;

        public RedisFileConfigurationRepository(ICacheClient cacheClient, IOptionsMonitor<RedisFileConfigurationOptions> options)
        {
            _cacheClient = cacheClient;
            _redisFileConfigurationOptions = options.CurrentValue;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            var fileConfiguration = await _cacheClient.GetAsync<FileConfiguration>(_redisFileConfigurationOptions.CacheName) ??
                                    new FileConfiguration();
            return new OkResponse<FileConfiguration>(fileConfiguration);
        }

        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            await _cacheClient.SetAsync(_redisFileConfigurationOptions.CacheName, fileConfiguration);
            return new OkResponse();
        }
    }
}
