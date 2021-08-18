namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户好友
    /// </summary>
    public partial class NChatBuddy
    {
        public string CbId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 好友ID
        /// </summary>
        public string CbUserId { get; set; }
        /// <summary>
        /// 归类ID（1：默认组，其它为引用）
        /// </summary>
        public string CcId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CbCreateTime { get; set; }
    }
}
