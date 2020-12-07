using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class UserReply
    {
        [Key]
        public int UrId { get; set; }
        public int? Uid { get; set; }
        [StringLength(20)]
        public string UrAnonymousName { get; set; }
        [StringLength(50)]
        public string UrAnonymousLink { get; set; }
        [StringLength(100)]
        public string UrAnonymousMail { get; set; }
        [StringLength(200)]
        public string UrTargetType { get; set; }
        [StringLength(50)]
        public string UrTargetId { get; set; }
        public string UrContent { get; set; }
        public string UrContentMd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UrCreateTime { get; set; }
        public int? UrStatus { get; set; }
        public int? UrTargetPid { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
