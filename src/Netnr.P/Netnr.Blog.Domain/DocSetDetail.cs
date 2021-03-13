using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(DsCode), Name = "IDXDocSetDetail_DsCode")]
    public partial class DocSetDetail
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string DsdId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DsdPid { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DsCode { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DsdTitle { get; set; }
        [Column(TypeName = "longtext")]
        public string DsdContentMd { get; set; }
        [Column(TypeName = "longtext")]
        public string DsdContentHtml { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsdCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsdUpdateTime { get; set; }
        public int? DsdOrder { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
        [Column(TypeName = "longtext")]
        public string DsdContent { get; set; }
        public int? DsdLetest { get; set; }
    }
}
