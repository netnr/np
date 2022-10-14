using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程节点
    /// </summary>
    public partial class WorkNode
    {
        /// <summary>
        /// 节点ID，唯一
        /// </summary>
        public long NodeId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 节点类型（关联字典 work_node_type）
        /// </summary>
        public string NodeType { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }
    }
}
