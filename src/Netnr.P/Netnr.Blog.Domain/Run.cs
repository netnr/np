using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(RunCode), Name = "IDXRun_RunCode")]
    [Index(nameof(Uid), Name = "IDXRun_Uid")]
    public partial class Run
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string RunId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string RunCode { get; set; }
        [Column(TypeName = "longtext")]
        public string RunContent1 { get; set; }
        [Column(TypeName = "longtext")]
        public string RunContent2 { get; set; }
        [Column(TypeName = "longtext")]
        public string RunContent3 { get; set; }
        [Column(TypeName = "longtext")]
        public string RunContent4 { get; set; }
        [Column(TypeName = "longtext")]
        public string RunContent5 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string RunTheme { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string RunRemark { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string RunTags { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RunCreateTime { get; set; }
        public int? RunOpen { get; set; }
        public int? RunStatus { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
