using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WeihanLi.Redis;

namespace WeihanLi.Configuration.Redis
{
    internal class RedisConfigurationProvider : ConfigurationProvider
    {
        private readonly IHashClient _hashClient;
        private readonly RedisConfigurationOption _redisOptions;
        private readonly string _configurationHashKey = "Configurations";

        public RedisConfigurationProvider(IHashClient hashClient, IOptions<RedisConfigurationOption> redisOptions)
        {
            _hashClient = hashClient;
            _redisOptions = redisOptions.Value;
        }

        public RedisConfigurationProvider(IHashClient hashClient, IOptions<RedisConfigurationOption> redisOptions, string configurationKey)
        {
            _hashClient = hashClient;
            _redisOptions = redisOptions.Value;
            if (!string.IsNullOrWhiteSpace(configurationKey))
            {
                _configurationHashKey = configurationKey;
            }
        }

        /// <summary>
        /// Loads the environment variables.
        /// </summary>
        public override void Load()
        {
            var keys = _hashClient.Keys(_configurationHashKey);
            if (keys.Length == 0)
                return;

            var configurations = keys
                .ToDictionary(_ => _, _ => _hashClient.Get(_configurationHashKey, _));
            if (_redisOptions.KeySeparator == ConfigurationPath.KeyDelimiter)
            {
                foreach (var entry in configurations)
                {
                    Data[entry.Key] = entry.Value;
                }
            }
            else
            {
                foreach (var entry in configurations)
                {
                    Data[entry.Key] = entry.Value.Replace(_redisOptions.KeySeparator, ConfigurationPath.KeyDelimiter);
                }
            }
        }
    }
}
