using System;
using System.Collections.Generic;

namespace Netnr.ResponseFramework.Domain.Entities
{
    /// <summary>
    /// 系统按钮表
    /// </summary>
    public partial class SysButton
    {
        public string SbId { get; set; }
        public string SbPid { get; set; }
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string SbBtnText { get; set; }
        /// <summary>
        /// 按钮ID
        /// </summary>
        public string SbBtnId { get; set; }
        /// <summary>
        /// 按钮类
        /// </summary>
        public string SbBtnClass { get; set; }
        /// <summary>
        /// 按钮图标
        /// </summary>
        public string SbBtnIcon { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SbBtnOrder { get; set; }
        /// <summary>
        /// 状态，1启用
        /// </summary>
        public int? SbStatus { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string SbDescribe { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public int? SbBtnGroup { get; set; }
        /// <summary>
        /// 隐藏，1隐藏
        /// </summary>
        public int? SbBtnHide { get; set; }
    }
}
