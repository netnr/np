using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "Draw_Uid")]
    public partial class Draw
    {
        [Key]
        [StringLength(50)]
        public string DrId { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string DrType { get; set; }
        [StringLength(50)]
        public string DrName { get; set; }
        public string DrContent { get; set; }
        [StringLength(200)]
        public string DrRemark { get; set; }
        [StringLength(20)]
        public string DrCategory { get; set; }
        public int? DrOrder { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DrCreateTime { get; set; }
        public int? DrStatus { get; set; }
        public int? DrOpen { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
