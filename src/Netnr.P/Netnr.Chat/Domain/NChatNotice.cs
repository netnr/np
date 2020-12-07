using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatNotice
    {
        [Key]
        [StringLength(50)]
        public string CnId { get; set; }
        [Required]
        [StringLength(50)]
        public string CuUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string CnFromId { get; set; }
        [Required]
        [StringLength(200)]
        public string CnNotice1 { get; set; }
        [StringLength(200)]
        public string CnNotice2 { get; set; }
        public int CnType { get; set; }
        public int? CnResult { get; set; }
        [StringLength(50)]
        public string CnReason { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CnCreateTime { get; set; }
        public int CnStatus { get; set; }
    }
}
