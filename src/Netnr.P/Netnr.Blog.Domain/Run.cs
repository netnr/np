using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(RunCode), Name = "Run_RunCode")]
    [Index(nameof(Uid), Name = "Run_Uid")]
    public partial class Run
    {
        [Key]
        [StringLength(50)]
        public string RunId { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string RunCode { get; set; }
        public string RunContent1 { get; set; }
        public string RunContent2 { get; set; }
        public string RunContent3 { get; set; }
        public string RunContent4 { get; set; }
        public string RunContent5 { get; set; }
        [StringLength(50)]
        public string RunTheme { get; set; }
        [StringLength(200)]
        public string RunRemark { get; set; }
        [StringLength(200)]
        public string RunTags { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RunCreateTime { get; set; }
        public int? RunOpen { get; set; }
        public int? RunStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
