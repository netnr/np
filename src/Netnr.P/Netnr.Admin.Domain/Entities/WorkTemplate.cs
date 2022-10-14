using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 工作流程模板
    /// </summary>
    public partial class WorkTemplate
    {
        /// <summary>
        /// 模板ID，唯一
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 模版类型（关联字典 work_template_type）
        /// </summary>
        public string TemplateType { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
    }
}
