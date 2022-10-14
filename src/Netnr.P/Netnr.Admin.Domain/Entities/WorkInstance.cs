using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程实例
    /// </summary>
    public partial class WorkInstance
    {
        /// <summary>
        /// 实例ID，唯一
        /// </summary>
        public long InstanceId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 关联模版ID
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 实例备注
        /// </summary>
        public string InstanceRemark { get; set; }
    }
}
