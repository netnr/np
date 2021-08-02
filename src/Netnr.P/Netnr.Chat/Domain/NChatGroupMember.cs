using System;

namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 组成员
    /// </summary>
    public partial class NChatGroupMember
    {
        public string CgmId { get; set; }
        /// <summary>
        /// 组ID
        /// </summary>
        public string CgId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public string CuMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CgmCreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常，2：禁言）
        /// </summary>
        public int CgmStatus { get; set; }
    }
}
