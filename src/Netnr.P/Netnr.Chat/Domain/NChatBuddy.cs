using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CuUserId), Name = "Index_1")]
    public partial class NChatBuddy
    {
        [Key]
        [StringLength(50)]
        public string CbId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CbUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CcId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CbCreateTime { get; set; }
    }
}
