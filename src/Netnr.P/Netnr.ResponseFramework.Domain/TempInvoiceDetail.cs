using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class TempInvoiceDetail
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string TidId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string TimId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string TimNo { get; set; }
        public int? TidOrder { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GoodsId { get; set; }
        public int? GoodsCount { get; set; }
        public decimal? GoodsCost { get; set; }
        public decimal? GoodsPrice { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
