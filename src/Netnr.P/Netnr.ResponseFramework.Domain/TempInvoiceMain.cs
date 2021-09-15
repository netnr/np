namespace Netnr.ResponseFramework.Domain
{
    /// <summary>
    /// 单据主表
    /// </summary>
    public partial class TempInvoiceMain
    {
        public string TimId { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string TimNo { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime? TimDate { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public string TimStore { get; set; }
        /// <summary>
        /// 采购类型
        /// </summary>
        public int? TimType { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string TimSupplier { get; set; }
        /// <summary>
        /// 采购员
        /// </summary>
        public string TimUser { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string TimRemark { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string TimOwnerId { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string TimOwnerName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? TimCreateTime { get; set; }
        /// <summary>
        /// 状态，1默认，2已审核，3未通过，4作废
        /// </summary>
        public int? TimStatus { get; set; }
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
