using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(GrCreateTime), Name = "IDXGuffRecord_GrCreateTime")]
    public partial class GuffRecord
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string GrId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GrTypeName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GrTypeValue { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GrObject { get; set; }
        [Column(TypeName = "text")]
        public string GrContent { get; set; }
        [Column(TypeName = "longtext")]
        public string GrContentMd { get; set; }
        [Column(TypeName = "text")]
        public string GrImage { get; set; }
        [Column(TypeName = "text")]
        public string GrAudio { get; set; }
        [Column(TypeName = "text")]
        public string GrVideo { get; set; }
        [Column(TypeName = "text")]
        public string GrFile { get; set; }
        [Column(TypeName = "text")]
        public string GrRemark { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GrTag { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrUpdateTime { get; set; }
        public int? GrReplyNum { get; set; }
        public int? GrOpen { get; set; }
        public int? GrReadNum { get; set; }
        public int? GrLaud { get; set; }
        public int? GrMark { get; set; }
        public int? GrStatus { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
