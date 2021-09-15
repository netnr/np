namespace Netnr.ResponseFramework.Domain
{
    /// <summary>
    /// 表配置
    /// </summary>
    public partial class SysTableConfig
    {
        public string Id { get; set; }
        /// <summary>
        /// （虚）表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 列键
        /// </summary>
        public string ColField { get; set; }
        /// <summary>
        /// 默认列标题
        /// </summary>
        public string DvTitle { get; set; }
        /// <summary>
        /// 列标题
        /// </summary>
        public string ColTitle { get; set; }
        /// <summary>
        /// 列宽
        /// </summary>
        public int? ColWidth { get; set; }
        /// <summary>
        /// 对齐方式 1左，2中，3右
        /// </summary>
        public int? ColAlign { get; set; }
        /// <summary>
        /// 1隐藏
        /// </summary>
        public int? ColHide { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? ColOrder { get; set; }
        /// <summary>
        /// 1冻结
        /// </summary>
        public int? ColFrozen { get; set; }
        /// <summary>
        /// 格式化
        /// </summary>
        public string ColFormat { get; set; }
        /// <summary>
        /// 1启用点击排序
        /// </summary>
        public int? ColSort { get; set; }
        /// <summary>
        /// 1导出
        /// </summary>
        public int? ColExport { get; set; }
        /// <summary>
        /// 1查询
        /// </summary>
        public int? ColQuery { get; set; }
        /// <summary>
        /// 查询关系符
        /// </summary>
        public string ColRelation { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int? FormArea { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string FormUrl { get; set; }
        /// <summary>
        /// 输入类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 跨列
        /// </summary>
        public int? FormSpan { get; set; }
        /// <summary>
        /// 1隐藏
        /// </summary>
        public int? FormHide { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? FormOrder { get; set; }
        /// <summary>
        /// 1必填
        /// </summary>
        public int? FormRequired { get; set; }
        /// <summary>
        /// 输入框提示
        /// </summary>
        public string FormPlaceholder { get; set; }
        /// <summary>
        /// 初始值
        /// </summary>
        public string FormValue { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        public string FormText { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int? FormMaxlength { get; set; }
    }
}
