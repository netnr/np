using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(GrId), nameof(Gid), Name = "IDXGiftRecordDetail_GrId_Gid")]
    public partial class GiftRecordDetail
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string GrdId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrdGiverName { get; set; }
        public decimal? GrdCash { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string GrdGoods { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrdCreateTime { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string GrdRemark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Gid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GiverName { get; set; }
        public decimal? GiftCash { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string GiftGoods { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
    }
}
