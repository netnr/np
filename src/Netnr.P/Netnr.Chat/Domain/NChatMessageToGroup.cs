namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 群组消息
    /// </summary>
    public partial class NChatMessageToGroup
    {
        public string CmgId { get; set; }
        /// <summary>
        /// 发送用户ID
        /// </summary>
        public string CmgPushUserId { get; set; }
        /// <summary>
        /// 发送用户设备
        /// </summary>
        public string CmgPushUserDevice { get; set; }
        /// <summary>
        /// 发送用户标识
        /// </summary>
        public string CmgPushUserSign { get; set; }
        /// <summary>
        /// 接收群组ID
        /// </summary>
        public string CmgPullGroupId { get; set; }
        /// <summary>
        /// 发送消息内容
        /// </summary>
        public string CmgContent { get; set; }
        /// <summary>
        /// 发送哪种
        /// </summary>
        public string CmgPushWhich { get; set; }
        /// <summary>
        /// 类型 枚举MessageType
        /// </summary>
        public string CmgPushType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CmgCreateTime { get; set; }
    }
}
