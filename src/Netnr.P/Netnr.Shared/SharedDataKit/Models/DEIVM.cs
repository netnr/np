#if Full || DataKit

using System;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 数据库环境信息
    /// </summary>
    public class DEIVM
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 编译
        /// </summary>
        public string Compile { get; set; }
        /// <summary>
        /// 安装目录
        /// </summary>
        public string DirInstall { get; set; }
        /// <summary>
        /// 数据库目录
        /// </summary>
        public string DirData { get; set; }
        /// <summary>
        /// 临时目录
        /// </summary>
        public string DirTemp { get; set; }
        /// <summary>
        /// 引擎
        /// </summary>
        public string Engine { get; set; }
        /// <summary>
        /// 字符集
        /// </summary>
        public string CharSet { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZone { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int? MaxConn { get; set; }
        /// <summary>
        /// 当前连接数
        /// </summary>
        public int? CurrConn { get; set; }
        /// <summary>
        /// 连接超时（秒）
        /// </summary>
        public int? TimeOut { get; set; }
        /// <summary>
        /// 忽略大小写（不区分大小写）
        /// </summary>
        public int? IgnoreCase { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string System { get; set; }
    }
}

#endif