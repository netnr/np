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
        public string DeiName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string DeiVersion { get; set; }
        /// <summary>
        /// 编译
        /// </summary>
        public string DeiCompile { get; set; }
        /// <summary>
        /// 安装目录
        /// </summary>
        public string DeiDirInstall { get; set; }
        /// <summary>
        /// 数据库目录
        /// </summary>
        public string DeiDirData { get; set; }
        /// <summary>
        /// 临时目录
        /// </summary>
        public string DeiDirTemp { get; set; }
        /// <summary>
        /// 引擎
        /// </summary>
        public string DeiEngine { get; set; }
        /// <summary>
        /// 字符集
        /// </summary>
        public string DeiCharSet { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        public string DeiTimeZone { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime DeiDateTime { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int? DeiMaxConn { get; set; }
        /// <summary>
        /// 当前连接数
        /// </summary>
        public int? DeiCurrConn { get; set; }
        /// <summary>
        /// 连接超时（秒）
        /// </summary>
        public int? DeiTimeout { get; set; }
        /// <summary>
        /// 忽略大小写（不区分大小写）
        /// </summary>
        public int? DeiIgnoreCase { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string DeiSystem { get; set; }
    }
}

#endif