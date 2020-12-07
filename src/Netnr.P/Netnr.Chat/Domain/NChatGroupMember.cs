using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CgId), Name = "Index_1")]
    [Index(nameof(CuUserId), Name = "Index_2")]
    public partial class NChatGroupMember
    {
        [Key]
        [StringLength(50)]
        public string CgmId { get; set; }
        [Required]
        [StringLength(50)]
        public string CgId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [StringLength(50)]
        public string CuMark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CgmCreateTime { get; set; }
        public int CgmStatus { get; set; }
    }
}
