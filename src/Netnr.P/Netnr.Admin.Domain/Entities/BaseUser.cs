using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public partial class BaseUser
    {
        /// <summary>
        /// 用户ID，唯一
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：正常；0：停用；2：禁止登录）
        /// </summary>
        public int Status { get; set; }
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
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string ActualName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
