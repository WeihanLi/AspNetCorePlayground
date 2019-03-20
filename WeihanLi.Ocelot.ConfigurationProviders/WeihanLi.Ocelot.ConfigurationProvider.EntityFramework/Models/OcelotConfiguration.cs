using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeihanLi.Ocelot.ConfigurationProvider.EntityFramework.Models
{
    /// <summary>
    /// Ocelot配置主表
    /// </summary>
    public class OcelotConfiguration
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 路由配置
        /// </summary>
        [NotMapped]
        public List<ReRoute> ReRoutes { get; set; } = new List<ReRoute>();

        /// <summary>
        /// 全局配置项
        /// </summary>
        [NotMapped]
        public GlobalConfiguration GlobalConfiguration { get; set; } = new GlobalConfiguration();
    }
}
