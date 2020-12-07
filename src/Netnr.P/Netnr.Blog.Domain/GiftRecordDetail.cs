using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(GrId), nameof(Gid), Name = "GiftRecordDetail_Gid")]
    public partial class GiftRecordDetail
    {
        [Key]
        [StringLength(50)]
        public string GrdId { get; set; }
        [StringLength(50)]
        public string GrId { get; set; }
        [StringLength(50)]
        public string GrdGiverName { get; set; }
        [Column(TypeName = "money")]
        public decimal? GrdCash { get; set; }
        [StringLength(255)]
        public string GrdGoods { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrdCreateTime { get; set; }
        [StringLength(255)]
        public string GrdRemark { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
        [StringLength(50)]
        public string Gid { get; set; }
        [StringLength(50)]
        public string GiverName { get; set; }
        [Column(TypeName = "money")]
        public decimal? GiftCash { get; set; }
        [StringLength(255)]
        public string GiftGoods { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
    }
}
