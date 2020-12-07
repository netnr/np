using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class TempInvoiceDetail
    {
        [Key]
        [StringLength(50)]
        public string TidId { get; set; }
        [StringLength(50)]
        public string TimId { get; set; }
        [StringLength(50)]
        public string TimNo { get; set; }
        public int? TidOrder { get; set; }
        [StringLength(50)]
        public string GoodsId { get; set; }
        public int? GoodsCount { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? GoodsCost { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? GoodsPrice { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
