namespace Netnr.Chat.Domain
{
    /// <summary>
    /// 用户表
    /// </summary>
    public partial class NChatUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CuUserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string CuUserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string CuUserNickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string CuPassword { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string CuUserPhoto { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CuCreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常，2：限制登录）
        /// </summary>
        public int CuStatus { get; set; }
    }
}
