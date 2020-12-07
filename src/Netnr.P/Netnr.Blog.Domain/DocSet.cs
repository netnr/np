using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "DocSet_Uid")]
    public partial class DocSet
    {
        [Key]
        [StringLength(50)]
        public string DsCode { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string DsName { get; set; }
        [StringLength(200)]
        public string DsRemark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsCreateTime { get; set; }
        public int? DsOpen { get; set; }
        public int? DsStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
