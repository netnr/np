using System;

namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 文件
    /// </summary>
    public partial class NChatFile
    {
        public string CfId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string CfFileName { get; set; }
        /// <summary>
        /// 文件全路径
        /// </summary>
        public string CfFullPath { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string CfType { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string CfExt { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long CfSize { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CfCreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常，2：限制）
        /// </summary>
        public int CfStatus { get; set; }
    }
}
