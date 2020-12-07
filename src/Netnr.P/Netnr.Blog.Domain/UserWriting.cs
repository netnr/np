using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "Writing_Uid")]
    public partial class UserWriting
    {
        [Key]
        public int UwId { get; set; }
        public int? Uid { get; set; }
        public int? UwCategory { get; set; }
        [StringLength(200)]
        public string UwTitle { get; set; }
        public string UwContent { get; set; }
        public string UwContentMd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UwCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UwUpdateTime { get; set; }
        public int? UwLastUid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UwLastDate { get; set; }
        public int? UwReplyNum { get; set; }
        public int? UwReadNum { get; set; }
        public int? UwOpen { get; set; }
        public int? UwLaud { get; set; }
        public int? UwMark { get; set; }
        public int? UwStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
