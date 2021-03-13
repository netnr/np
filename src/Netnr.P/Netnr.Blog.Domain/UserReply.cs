using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(UrTargetType), nameof(UrTargetId), Name = "IDXUserReply_UrTargetType_UrTargetId")]
    public partial class UserReply
    {
        [Key]
        public int UrId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string UrAnonymousName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UrAnonymousLink { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string UrAnonymousMail { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UrTargetType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UrTargetId { get; set; }
        [Column(TypeName = "longtext")]
        public string UrContent { get; set; }
        [Column(TypeName = "longtext")]
        public string UrContentMd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UrCreateTime { get; set; }
        public int? UrStatus { get; set; }
        public int? UrTargetPid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
