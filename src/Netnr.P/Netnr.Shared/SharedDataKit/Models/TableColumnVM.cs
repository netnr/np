#if Full || DataKit

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 表列信息
    /// </summary>
    public partial class TableColumnVM
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableComment { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 数据类型及长度
        /// </summary>
        public string DataTypeLength { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public string DataLength { get; set; }
        /// <summary>
        /// 数据精度
        /// </summary>
        public string DataScale { get; set; }
        /// <summary>
        /// 字段排序
        /// </summary>
        public int? FieldOrder { get; set; }
        /// <summary>
        /// 主键（YES：是主键）
        /// </summary>
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 自增（YES：是自增）
        /// </summary>
        public string AutoAdd { get; set; }
        /// <summary>
        /// 为空（YES：不为空）
        /// </summary>
        public string NotNull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string FieldComment { get; set; }
    }
}

#endif