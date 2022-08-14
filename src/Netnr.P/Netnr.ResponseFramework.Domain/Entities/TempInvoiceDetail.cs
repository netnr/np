using System;
using System.Collections.Generic;

namespace Netnr.ResponseFramework.Domain.Entities
{
    /// <summary>
    /// 单据明细
    /// </summary>
    public partial class TempInvoiceDetail
    {
        public string TidId { get; set; }
        /// <summary>
        /// 单据主表ID
        /// </summary>
        public string TimId { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string TimNo { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? TidOrder { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string GoodsId { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int? GoodsCount { get; set; }
        /// <summary>
        /// 商品成本
        /// </summary>
        public decimal? GoodsCost { get; set; }
        /// <summary>
        /// 商品售价
        /// </summary>
        public decimal? GoodsPrice { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string Spare1 { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string Spare2 { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string Spare3 { get; set; }
    }
}
