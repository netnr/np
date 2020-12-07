using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Chat.Domain
{
    [Index(nameof(CuUserId), Name = "Index_1")]
    public partial class NChatClassify
    {
        [Key]
        [StringLength(50)]
        public string CcId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CcName { get; set; }
        public int CcType { get; set; }
        public int CcOrder { get; set; }
    }
}
