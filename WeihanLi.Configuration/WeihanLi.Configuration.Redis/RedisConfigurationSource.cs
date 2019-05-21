using Microsoft.Extensions.Configuration;

namespace WeihanLi.Configuration.Redis
{
    public class RedisConfigurationSource : IConfigurationSource
    {
        public string ConfigurationKey { get; set; } = "Configurations";

        public string KeyDelimiter { get; set; } = ":";

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RedisConfigurationProvider(ConfigurationKey, KeyDelimiter);
        }
    }
}
