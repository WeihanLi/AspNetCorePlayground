using System;

namespace WeihanLi.Ocelot.ConfigurationProvider.Redis
{
    public class RedisFileConfigurationOptions
    {
        /// <summary>
        /// redis cache name
        /// </summary>
        public string CacheName { get; set; } = "OcelotConfigurations";

        /// <summary>
        /// 默认过期时间
        /// </summary>
        public TimeSpan? DefaultExpiresIn { get; set; } = TimeSpan.FromDays(30);
    }
}
