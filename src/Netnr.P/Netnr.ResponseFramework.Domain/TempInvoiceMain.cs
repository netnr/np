using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class TempInvoiceMain
    {
        [Key]
        [StringLength(50)]
        public string TimId { get; set; }
        [StringLength(50)]
        public string TimNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TimDate { get; set; }
        [StringLength(50)]
        public string TimStore { get; set; }
        public int? TimType { get; set; }
        [StringLength(50)]
        public string TimSupplier { get; set; }
        [StringLength(50)]
        public string TimUser { get; set; }
        [StringLength(200)]
        public string TimRemark { get; set; }
        [StringLength(50)]
        public string TimOwnerId { get; set; }
        [StringLength(20)]
        public string TimOwnerName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TimCreateTime { get; set; }
        public int? TimStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
