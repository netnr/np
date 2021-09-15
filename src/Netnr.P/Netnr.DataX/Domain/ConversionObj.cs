namespace Netnr.DataX.Domain
{
    /// <summary>
    /// 转换对象
    /// </summary>
    public class ConversionObj
    {
        /// <summary>
        /// 原库表名
        /// </summary>
        public string OdTableName { get; set; }
        /// <summary>
        /// 原库表数据查询SQL
        /// </summary>
        public string OdQuerySql { get; set; }
        /// <summary>
        /// 新库表名
        /// </summary>
        public string NdTableName { get; set; }
        /// <summary>
        /// 新库表数据清理（为空不清理）
        /// </summary>
        public string NdClearTableSql { get; set; }
    }
}
