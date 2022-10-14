using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程实例步骤
    /// </summary>
    public partial class WorkInstanceStep
    {
        /// <summary>
        /// 步骤ID，唯一
        /// </summary>
        public long StepId { get; set; }
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
        /// 关联实例ID
        /// </summary>
        public long InstanceId { get; set; }
        /// <summary>
        /// 关联模板节点ID
        /// </summary>
        public long TnodeId { get; set; }
        /// <summary>
        /// 关联节点ID
        /// </summary>
        public long NodeId { get; set; }
    }
}
