namespace Netnr.ResponseFramework.Domain
{
    /// <summary>
    /// 系统日志表
    /// </summary>
    public partial class SysLog
    {
        public string LogId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string SuName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string SuNickname { get; set; }
        /// <summary>
        /// 动作
        /// </summary>
        public string LogAction { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string LogContent { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string LogUrl { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string LogIp { get; set; }
        /// <summary>
        /// User-Agent
        /// </summary>
        public string LogUserAgent { get; set; }
        /// <summary>
        /// 浏览器
        /// </summary>
        public string LogBrowserName { get; set; }
        /// <summary>
        /// 客户端操作系统
        /// </summary>
        public string LogSystemName { get; set; }
        /// <summary>
        /// 分组（1：默认；2：爬虫）
        /// </summary>
        public int? LogGroup { get; set; }
        /// <summary>
        /// 级别（F： Fatal；E：Error；W：Warn；I：Info；D：Debug；A：All）
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? LogCreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string LogRemark { get; set; }
    }
}
