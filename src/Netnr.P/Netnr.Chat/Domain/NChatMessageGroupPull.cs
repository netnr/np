using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatMessageGroupPull
    {
        [Required]
        [StringLength(50)]
        public string GpId { get; set; }
        [Key]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CgGroupId { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime GpUpdateTime { get; set; }
    }
}
