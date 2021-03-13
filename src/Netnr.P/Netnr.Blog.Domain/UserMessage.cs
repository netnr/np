using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), nameof(UmType), nameof(UmCreateTime), Name = "IDXUserMessage_Uid_UmType_UmCreateTime")]
    public partial class UserMessage
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string UmId { get; set; }
        public int? Uid { get; set; }
        public int? UmTriggerUid { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UmType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UmTargetId { get; set; }
        public int? UmTargetIndex { get; set; }
        public int? UmAction { get; set; }
        [Column(TypeName = "longtext")]
        public string UmContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UmCreateTime { get; set; }
        public int? UmStatus { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
