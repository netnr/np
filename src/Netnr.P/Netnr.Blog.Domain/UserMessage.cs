using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class UserMessage
    {
        [Key]
        [StringLength(50)]
        public string UmId { get; set; }
        public int? Uid { get; set; }
        public int? UmTriggerUid { get; set; }
        [StringLength(200)]
        public string UmType { get; set; }
        [StringLength(50)]
        public string UmTargetId { get; set; }
        public int? UmTargetIndex { get; set; }
        public int? UmAction { get; set; }
        public string UmContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UmCreateTime { get; set; }
        public int? UmStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
