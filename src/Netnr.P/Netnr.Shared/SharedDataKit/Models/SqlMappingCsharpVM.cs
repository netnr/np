#if Full || DataKit

using System;

namespace Netnr.SharedDataKit
{
    public class SqlMappingCsharpVM
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 数据类型（C#）
        /// </summary>
        public string DataTypeName { get; set; }
        /// <summary>
        /// 数据类型（DB）
        /// </summary>
        public Enum DbType { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 允许为Null
        /// </summary>
        public bool AllowDBNull { get; set; }
        /// <summary>
        /// 字段排序
        /// </summary>
        public int Ordinal { get; set; }
    }
}

#endif