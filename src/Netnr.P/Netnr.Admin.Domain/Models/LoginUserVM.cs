namespace Netnr.Admin.Domain.Models
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class LoginUserVM
    {
        /// <summary>
        /// 用户ID，唯一
        /// </summary>
        public long UserId { get; set; } = 0;
        /// <summary>
        /// 关联角色ID（多选逗号分割）
        /// </summary>
        public string RolesId { get; set; }
        /// <summary>
        /// 关联组织架构ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 账号，唯一
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Authorization 有效期时间戳（单位秒）
        /// </summary>
        public long Expired { get; set; }
    }
}
