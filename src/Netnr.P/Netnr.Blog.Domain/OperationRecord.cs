using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class OperationRecord
    {
        [Key]
        [StringLength(50)]
        public string OrId { get; set; }
        [StringLength(200)]
        public string OrType { get; set; }
        [StringLength(50)]
        public string OrAction { get; set; }
        [StringLength(2000)]
        public string OrSource { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrCreateTime { get; set; }
        [StringLength(50)]
        public string OrMark { get; set; }
        [StringLength(50)]
        public string OrRemark { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
