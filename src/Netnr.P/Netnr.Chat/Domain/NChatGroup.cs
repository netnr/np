using System;

namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户组
    /// </summary>
    public partial class NChatGroup
    {
        public string CgId { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string CgName { get; set; }
        /// <summary>
        /// 所属用户ID
        /// </summary>
        public string CgOwnerId { get; set; }
        /// <summary>
        /// 归类ID（1：默认组，其它为引用）
        /// </summary>
        public string CcId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CgCreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常）
        /// </summary>
        public int CgStatus { get; set; }
    }
}
