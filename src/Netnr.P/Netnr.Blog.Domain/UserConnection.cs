using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXUserConnection_Uid")]
    public partial class UserConnection
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string UconnId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UconnTargetType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UconnTargetId { get; set; }
        public int? UconnAction { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UconnCreateTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
