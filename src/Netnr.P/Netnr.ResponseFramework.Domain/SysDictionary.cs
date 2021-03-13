using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    [Index(nameof(SdType), Name = "IDXSysDictionary_SdType")]
    public partial class SysDictionary
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string SdId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SdPid { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SdType { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SdKey { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SdValue { get; set; }
        public int? SdOrder { get; set; }
        public int? SdStatus { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SdRemark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SdAttribute1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SdAttribute2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SdAttribute3 { get; set; }
    }
}
