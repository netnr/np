using System;
using System.Collections.Generic;

namespace Netnr.Admin.Domain.Entities
{
    /// <summary>
    /// 字典
    /// </summary>
    public partial class BaseDictionary
    {
        /// <summary>
        /// 字典ID，唯一
        /// </summary>
        public long DictId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态（1：启用；0：停用）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 字典父级ID，无父级为0
        /// </summary>
        public long DictPid { get; set; }
        /// <summary>
        /// 字典类别
        /// </summary>
        public string DictType { get; set; }
        /// <summary>
        /// 字典键
        /// </summary>
        public string DictKey { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string DictValue { get; set; }
        /// <summary>
        /// 字典排序
        /// </summary>
        public int DictOrder { get; set; }
        /// <summary>
        /// 字典备注
        /// </summary>
        public string DictRemark { get; set; }
    }
}
