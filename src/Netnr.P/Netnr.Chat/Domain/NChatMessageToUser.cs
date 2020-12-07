using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CmuPushUserId), Name = "Index_1")]
    [Index(nameof(CmuPullUserId), Name = "Index_2")]
    public partial class NChatMessageToUser
    {
        [Key]
        [StringLength(50)]
        public string CmuId { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPushUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPushUserDevice { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPushUserSign { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPullUserId { get; set; }
        [Required]
        public string CmuContent { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPushWhich { get; set; }
        [Required]
        [StringLength(50)]
        public string CmuPushType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CmuCreateTime { get; set; }
        public int CmuStatus { get; set; }
    }
}
