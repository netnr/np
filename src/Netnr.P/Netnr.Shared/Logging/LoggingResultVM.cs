﻿#if Full || Logging

namespace Netnr
{
    /// <summary>
    /// 查询返回
    /// </summary>
    public class LoggingResultVM
    {
        /// <summary>
        /// 总条数（仅对分页查询时有效）
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// 丢失的库（附加库有上限会丢失数据库）
        /// </summary>
        public int Lost { get; set; }
        /// <summary>
        /// 数据体
        /// </summary>
        public object RowData { get; set; }
    }
}

#endif