namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户、组归类
    /// </summary>
    public partial class NChatClassify
    {
        public string CcId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 归类名称
        /// </summary>
        public string CcName { get; set; }
        /// <summary>
        /// 归类类型（1：用户好友，2：用户组）
        /// </summary>
        public int CcType { get; set; }
        /// <summary>
        /// 归类排序
        /// </summary>
        public int CcOrder { get; set; }
    }
}
