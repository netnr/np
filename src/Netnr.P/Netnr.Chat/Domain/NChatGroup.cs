using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatGroup
    {
        [Key]
        [StringLength(50)]
        public string CgId { get; set; }
        [Required]
        [StringLength(50)]
        public string CgName { get; set; }
        [Required]
        [StringLength(50)]
        public string CgOwnerId { get; set; }
        [Required]
        [StringLength(50)]
        public string CcId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CgCreateTime { get; set; }
        public int CgStatus { get; set; }
    }
}
