using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXDraw_Uid")]
    public partial class Draw
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string DrId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DrType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DrName { get; set; }
        [Column(TypeName = "longtext")]
        public string DrContent { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string DrRemark { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string DrCategory { get; set; }
        public int? DrOrder { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DrCreateTime { get; set; }
        public int? DrStatus { get; set; }
        public int? DrOpen { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
