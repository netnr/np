using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程节点表单
    /// </summary>
    public partial class WorkNodeForm
    {
        /// <summary>
        /// 表单ID，唯一
        /// </summary>
        public long FormId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 关联节点ID
        /// </summary>
        public long NodeId { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// 表单KEY
        /// </summary>
        public string FormKey { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单排序
        /// </summary>
        public int FormOrder { get; set; }
        /// <summary>
        /// 表单备注
        /// </summary>
        public string FormRemark { get; set; }
    }
}
