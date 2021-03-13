using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXDocSet_Uid")]
    public partial class DocSet
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string DsCode { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string DsName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string DsRemark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DsCreateTime { get; set; }
        public int? DsOpen { get; set; }
        public int? DsStatus { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
