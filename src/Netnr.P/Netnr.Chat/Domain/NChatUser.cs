using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CuUserName), Name = "Index_1")]
    public partial class NChatUser
    {
        [Key]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserName { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserNickname { get; set; }
        [Required]
        [StringLength(50)]
        public string CuPassword { get; set; }
        [Required]
        [StringLength(200)]
        public string CuUserPhoto { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CuCreateTime { get; set; }
        public int CuStatus { get; set; }
    }
}
