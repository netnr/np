using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 按钮
    /// </summary>
    public partial class BaseButton
    {
        /// <summary>
        /// 主键，唯一
        /// </summary>
        public long ButtonId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string ButtonText { get; set; }
        /// <summary>
        /// 按钮标识
        /// </summary>
        public string ButtonKey { get; set; }
        /// <summary>
        /// 按钮类名
        /// </summary>
        public string ButtonClass { get; set; }
        /// <summary>
        /// 按钮图标
        /// </summary>
        public string ButtonIcon { get; set; }
        /// <summary>
        /// 按钮排序
        /// </summary>
        public int ButtonOrder { get; set; }
        /// <summary>
        /// 按钮分组（1：默认）
        /// </summary>
        public int ButtonGroup { get; set; }
        /// <summary>
        /// 按钮备注
        /// </summary>
        public string ButtonRemark { get; set; }
    }
}
