namespace Netnr.Blog.Domain.Models
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class LoginUserVM
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 登录标记
        /// </summary>
        public string UserSign { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// Authorization 有效期时间戳（单位秒）
        /// </summary>
        public long Expired { get; set; }
    }
}
