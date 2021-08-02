using System;

namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户消息组接收记录
    /// </summary>
    public partial class NChatMessageGroupPull
    {
        public string GpId { get; set; }
        /// <summary>
        /// 接收用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 组ID
        /// </summary>
        public string CgGroupId { get; set; }
        /// <summary>
        /// 组消息ID
        /// </summary>
        public string CmgId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime GpUpdateTime { get; set; }
    }
}
