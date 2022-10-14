using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 菜单
    /// </summary>
    public partial class BaseMenu
    {
        /// <summary>
        /// 菜单ID，唯一
        /// </summary>
        public long MenuId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 菜单父级ID，无父级为0
        /// </summary>
        public long MenuPid { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单链接
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }
        /// <summary>
        /// 菜单分组（1：PC；2：Mobile）
        /// </summary>
        public int MenuGroup { get; set; }
        /// <summary>
        /// 菜单排序
        /// </summary>
        public int MenuOrder { get; set; }
        /// <summary>
        /// 菜单备注
        /// </summary>
        public string MenuRemark { get; set; }
    }
}
