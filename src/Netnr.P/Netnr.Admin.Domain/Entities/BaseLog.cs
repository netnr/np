using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 日志
    /// </summary>
    public partial class BaseLog
    {
        /// <summary>
        /// 日志ID，唯一
        /// </summary>
        public long LogId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 关联用户
        /// </summary>
        public string LogUser { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string LogNickname { get; set; }
        /// <summary>
        /// 类型（-1：异常；1：默认；2：登录；）
        /// </summary>
        public int LogType { get; set; }
        /// <summary>
        /// 动作
        /// </summary>
        public string LogAction { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string LogUrl { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string LogContent { get; set; }
        /// <summary>
        /// 执行 SQL
        /// </summary>
        public string LogSql { get; set; }
        /// <summary>
        /// 用时（毫秒）
        /// </summary>
        public long? LogTimeCost { get; set; }
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
        public string LogBrowser { get; set; }
        /// <summary>
        /// 系统
        /// </summary>
        public string LogSystem { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string LogRemark { get; set; }
    }
}
