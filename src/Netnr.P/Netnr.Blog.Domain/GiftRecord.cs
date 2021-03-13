using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXGiftRecord_Uid")]
    public partial class GiftRecord
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string GrId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Uid { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GrTheme { get; set; }
        public int? GrType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrName1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrName2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrName3 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GrName4 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrActionTime { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string GrDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GrCreateTime { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string GrRemark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Theme { get; set; }
        public int? Type { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name3 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name4 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionTime { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Describe { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Remark { get; set; }
    }
}
