using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CmgPushUserId), Name = "Index_1")]
    [Index(nameof(CmgPullGroupId), Name = "Index_2")]
    public partial class NChatMessageToGroup
    {
        [Key]
        [StringLength(50)]
        public string CmgId { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPushUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPushUserDevice { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPushUserSign { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPullGroupId { get; set; }
        [Required]
        public string CmgContent { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPushWhich { get; set; }
        [Required]
        [StringLength(50)]
        public string CmgPushType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CmgCreateTime { get; set; }
    }
}
