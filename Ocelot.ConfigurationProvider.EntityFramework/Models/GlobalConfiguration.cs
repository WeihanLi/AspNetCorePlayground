namespace Ocelot.ConfigurationProvider.EntityFramework.Models
{
    /// <summary>
    /// 全局配置项
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// OcelotConfigurationId
        /// </summary>
        public int ConfigurationId { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string RequestIdKey { get; set; } = "OcelotRequestId";

        /// <summary>
        /// 基Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 下游服务请求类型，http or https
        /// </summary>
        public string DownstreamScheme { get; set; } = "http";

        /// <summary>
        /// HttpHandler配置项
        /// </summary>
        public string HttpHandlerOption { get; set; }
    }
}
