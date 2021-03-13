using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysRole
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string SrId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SrName { get; set; }
        public int? SrStatus { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SrDescribe { get; set; }
        public int? SrGroup { get; set; }
        [Column(TypeName = "text")]
        public string SrMenus { get; set; }
        [Column(TypeName = "text")]
        public string SrButtons { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SrCreateTime { get; set; }
    }
}
