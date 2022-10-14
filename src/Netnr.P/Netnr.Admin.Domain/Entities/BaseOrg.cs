using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 组织架构
    /// </summary>
    public partial class BaseOrg
    {
        /// <summary>
        /// 组织架构ID，唯一
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 组织架构父级ID（无父级为0）
        /// </summary>
        public long OrgPid { get; set; }
        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 组织代码
        /// </summary>
        public string OrgCode { get; set; }
        /// <summary>
        /// 组织备注
        /// </summary>
        public string OrgRemark { get; set; }
    }
}
