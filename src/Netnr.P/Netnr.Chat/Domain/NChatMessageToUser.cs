namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户消息
    /// </summary>
    public partial class NChatMessageToUser
    {
        public string CmuId { get; set; }
        /// <summary>
        /// 发送用户ID
        /// </summary>
        public string CmuPushUserId { get; set; }
        /// <summary>
        /// 发送用户设备
        /// </summary>
        public string CmuPushUserDevice { get; set; }
        /// <summary>
        /// 发送用户标识
        /// </summary>
        public string CmuPushUserSign { get; set; }
        /// <summary>
        /// 接收用户ID
        /// </summary>
        public string CmuPullUserId { get; set; }
        /// <summary>
        /// 发送消息内容
        /// </summary>
        public string CmuContent { get; set; }
        /// <summary>
        /// 发送哪种
        /// </summary>
        public string CmuPushWhich { get; set; }
        /// <summary>
        /// 消息类型 枚举MessageType
        /// </summary>
        public string CmuPushType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CmuCreateTime { get; set; }
        /// <summary>
        /// 消息状态（-1：撤回，1：待推送，2：已推送，3：已接收，4：已读）
        /// </summary>
        public int CmuStatus { get; set; }
    }
}
