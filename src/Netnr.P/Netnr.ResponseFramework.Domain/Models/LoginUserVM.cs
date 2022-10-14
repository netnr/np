namespace Netnr.ResponseFramework.Domain.Models
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class LoginUserVM
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// Authorization 有效期时间戳（单位秒）
        /// </summary>
        public long Expired { get; set; }
    }
}
