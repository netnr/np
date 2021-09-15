namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 通知信息
    /// </summary>
    public partial class NChatNotice
    {
        public string CnId { get; set; }
        /// <summary>
        /// 接收用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 发送用户ID
        /// </summary>
        public string CnFromId { get; set; }
        /// <summary>
        /// 发送通知
        /// </summary>
        public string CnNotice1 { get; set; }
        /// <summary>
        /// 发送通知
        /// </summary>
        public string CnNotice2 { get; set; }
        /// <summary>
        /// 类型 枚举MessageType
        /// </summary>
        public string CnType { get; set; }
        /// <summary>
        /// 接收用户处理结果（1：通过，2：拒绝）
        /// </summary>
        public int? CnResult { get; set; }
        /// <summary>
        /// 接收用户处理事由
        /// </summary>
        public string CnReason { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CnCreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常，2：删除）
        /// </summary>
        public int CnStatus { get; set; }
    }
}
