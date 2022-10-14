using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public partial class BaseRole
    {
        /// <summary>
        /// 角色ID，唯一
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 关联菜单（逗号分割）
        /// </summary>
        public string RoleMenus { get; set; }
        /// <summary>
        /// 关联按钮（JSON）
        /// </summary>
        public string RoleButtons { get; set; }
        /// <summary>
        /// 角色分组（默认1）
        /// </summary>
        public int RoleGroup { get; set; }
    }
}
