namespace Ocelot.ConfigurationProvider.EntityFramework.Models
{
    /// <summary>
    /// 路由配置项
    /// </summary>
    public class ReRoute
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
        /// 下游服务地址模板
        /// </summary>
        public string DownstreamPathTemplate { get; set; }

        /// <summary>
        /// 上游服务地址模板
        /// </summary>
        public string UpstreamPathTemplate { get; set; }

        /// <summary>
        /// 上游服务请求类型
        /// </summary>
        public string UpstreamHttpMethod { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public string AddHeadersToRequest { get; set; }

        /// <summary>
        /// 上游服务请求头转换
        /// </summary>
        public string UpstreamHeaderTransform { get; set; }

        /// <summary>
        /// 下游服务请求头转换
        /// </summary>
        public string DownstreamHeaderTransform { get; set; }

        /// <summary>
        /// 为请求添加Claims
        /// </summary>
        public string AddClaimsToRequest { get; set; }

        /// <summary>
        /// 路由中必须存在的Claims
        /// </summary>
        public string RouteClaimsRequirement { get; set; }

        /// <summary>
        /// 向请求添加查询字符串
        /// </summary>
        public string AddQueriesToRequest { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestIdKey { get; set; } = "OcelotRequestId";

        /// <summary>
        /// 缓存配置
        /// </summary>
        public string CacheOption { get; set; }

        /// <summary>
        /// 路由是否区分大小写
        /// </summary>
        public bool ReRouteIsCaseSensitive { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 下游服务请求类别，http or https
        /// </summary>
        public string DownstreamScheme { get; set; } = "http";

        /// <summary>
        /// 服务质量配置
        /// </summary>
        public string QoSOption { get; set; }

        /// <summary>
        /// 轮询策略配置
        /// </summary>
        public string LoadBalancerOption { get; set; }

        /// <summary>
        /// 限流配置
        /// </summary>
        public string RateLimitOption { get; set; }

        /// <summary>
        /// 认证配置
        /// </summary>
        public string AuthenticationOption { get; set; }

        /// <summary>
        /// HttpHandler配置
        /// </summary>
        public string HttpHandlerOption { get; set; }

        /// <summary>
        /// 下游服务列表
        /// </summary>
        public string DownstreamHostAndPorts { get; set; }

        /// <summary>
        /// 上游服务地址
        /// </summary>
        public string UpstreamHost { get; set; }

        /// <summary>
        /// 路由唯一键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 委托处理
        /// </summary>
        public string DelegatingHandler { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 是否忽略SSL警告
        /// </summary>
        public bool DangerousAcceptAnyServerCertificateValidator { get; set; }

        /// <summary>
        /// 安全配置项
        /// </summary>
        public string SecurityOption { get; set; }
    }
}
