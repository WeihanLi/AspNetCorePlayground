namespace WeihanLi.Ocelot.ConfigurationProvider.Redis
{
    public class RedisFileConfigurationOptions
    {
        /// <summary>
        /// redis cache name
        /// </summary>
        public string CacheName { get; set; } = "OcelotConfigurations";
    }
}
