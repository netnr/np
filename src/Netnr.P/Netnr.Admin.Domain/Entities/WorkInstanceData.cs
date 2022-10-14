using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程实例数据
    /// </summary>
    public partial class WorkInstanceData
    {
        /// <summary>
        /// 数据ID，唯一
        /// </summary>
        public long DataId { get; set; }
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
        /// <summary>
        /// 关联表单ID
        /// </summary>
        public long FormId { get; set; }
        /// <summary>
        /// 关联步骤ID
        /// </summary>
        public long StepId { get; set; }
        /// <summary>
        /// 表单值
        /// </summary>
        public string DataValue { get; set; }
    }
}
