using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatFile
    {
        [Key]
        [StringLength(50)]
        public string CfId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(500)]
        public string CfFileName { get; set; }
        [Required]
        [StringLength(1000)]
        public string CfFullPath { get; set; }
        [Required]
        [StringLength(100)]
        public string CfType { get; set; }
        [Required]
        [StringLength(20)]
        public string CfExt { get; set; }
        public long CfSize { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CfCreateTime { get; set; }
        public int CfStatus { get; set; }
    }
}
