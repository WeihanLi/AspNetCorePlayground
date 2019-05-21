using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Redis;

namespace WeihanLi.Configuration.Redis
{
    internal class RedisConfigurationProvider : ConfigurationProvider
    {
        private readonly string _configurationHashKey;
        private readonly string _keyDelimiter;

        public RedisConfigurationProvider(string configurationKey, string keyDelimiter)
        {
            _configurationHashKey = configurationKey;
            _keyDelimiter = keyDelimiter;
        }

        /// <summary>
        /// Loads the environment variables.
        /// </summary>
        public override void Load()
        {
            var hashClient = DependencyResolver.Current.GetRequiredService<IHashClient>();
            var keys = hashClient.Keys(_configurationHashKey);
            if (keys.Length == 0)
                return;

            var configurations = keys
                .ToDictionary(_ => _, _ => hashClient.Get(_configurationHashKey, _));
            if (_keyDelimiter == ConfigurationPath.KeyDelimiter)
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
                    Data[entry.Key.Replace(_keyDelimiter, ConfigurationPath.KeyDelimiter)] = entry.Value;
                }
            }
        }
    }
}
