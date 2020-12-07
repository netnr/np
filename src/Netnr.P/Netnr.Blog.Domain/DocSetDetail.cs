using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class DocSetDetail
    {
        [Key]
        [StringLength(50)]
        public string DsdId { get; set; }
        [StringLength(50)]
        public string DsdPid { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string DsCode { get; set; }
        [StringLength(50)]
        public string DsdTitle { get; set; }
        public string DsdContentMd { get; set; }
        public string DsdContentHtml { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsdCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsdUpdateTime { get; set; }
        public int? DsdOrder { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
        public string DsdContent { get; set; }
        public int? DsdLetest { get; set; }
    }
}
