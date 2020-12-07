using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class UserConnection
    {
        [Key]
        [StringLength(50)]
        public string UconnId { get; set; }
        public int? Uid { get; set; }
        [StringLength(200)]
        public string UconnTargetType { get; set; }
        [StringLength(50)]
        public string UconnTargetId { get; set; }
        public int? UconnAction { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UconnCreateTime { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
