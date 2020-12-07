using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysRole
    {
        [Key]
        [StringLength(50)]
        public string SrId { get; set; }
        [StringLength(200)]
        public string SrName { get; set; }
        public int? SrStatus { get; set; }
        [StringLength(200)]
        public string SrDescribe { get; set; }
        public int? SrGroup { get; set; }
        public string SrMenus { get; set; }
        public string SrButtons { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SrCreateTime { get; set; }
    }
}
