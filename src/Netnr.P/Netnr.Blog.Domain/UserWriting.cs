using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXUserWriting_Uid")]
    public partial class UserWriting
    {
        [Key]
        public int UwId { get; set; }
        public int? Uid { get; set; }
        public int? UwCategory { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UwTitle { get; set; }
        [Column(TypeName = "longtext")]
        public string UwContent { get; set; }
        [Column(TypeName = "longtext")]
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
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
