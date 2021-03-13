using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class OperationRecord
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string OrId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string OrType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OrAction { get; set; }
        [Column(TypeName = "text")]
        public string OrSource { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrCreateTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OrMark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OrRemark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
