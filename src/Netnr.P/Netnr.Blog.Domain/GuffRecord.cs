using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class GuffRecord
    {
        [Key]
        [StringLength(50)]
        public string GrId { get; set; }
        public int? Uid { get; set; }
        [StringLength(200)]
        public string GrTypeName { get; set; }
        [StringLength(200)]
        public string GrTypeValue { get; set; }
        [StringLength(200)]
        public string GrObject { get; set; }
        [StringLength(4000)]
        public string GrContent { get; set; }
        public string GrContentMd { get; set; }
        [StringLength(4000)]
        public string GrImage { get; set; }
        [StringLength(4000)]
        public string GrAudio { get; set; }
        [StringLength(4000)]
        public string GrVideo { get; set; }
        [StringLength(4000)]
        public string GrFile { get; set; }
        [StringLength(4000)]
        public string GrRemark { get; set; }
        [StringLength(200)]
        public string GrTag { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrUpdateTime { get; set; }
        public int? GrReplyNum { get; set; }
        public int? GrOpen { get; set; }
        public int? GrReadNum { get; set; }
        public int? GrLaud { get; set; }
        public int? GrMark { get; set; }
        public int? GrStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
