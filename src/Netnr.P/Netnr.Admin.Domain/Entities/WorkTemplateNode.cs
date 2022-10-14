using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程模版节点
    /// </summary>
    public partial class WorkTemplateNode
    {
        /// <summary>
        /// 模板节点ID，唯一
        /// </summary>
        public long TnodeId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 关联模板ID
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 关联节点ID
        /// </summary>
        public long NodeId { get; set; }
        /// <summary>
        /// 模板节点名称（默认继承节点名称）
        /// </summary>
        public string TnodeName { get; set; }
        /// <summary>
        /// 下一模版节点ID，多选
        /// </summary>
        public string TnodeNextIds { get; set; }
        /// <summary>
        /// 模版节点要求
        /// </summary>
        public string TnodeRequest { get; set; }
        /// <summary>
        /// 模版节点关联人ID
        /// </summary>
        public string TnodePerson { get; set; }
    }
}
