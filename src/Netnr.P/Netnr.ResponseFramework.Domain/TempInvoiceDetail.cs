namespace Netnr.ResponseFramework.Domain
{
    public partial class TempInvoiceDetail
    {
        public string TidId { get; set; }
        public string TimId { get; set; }
        public string TimNo { get; set; }
        public int? TidOrder { get; set; }
        public string GoodsId { get; set; }
        public int? GoodsCount { get; set; }
        public decimal? GoodsCost { get; set; }
        public decimal? GoodsPrice { get; set; }
        public string Spare1 { get; set; }
        public string Spare2 { get; set; }
        public string Spare3 { get; set; }
    }
}
