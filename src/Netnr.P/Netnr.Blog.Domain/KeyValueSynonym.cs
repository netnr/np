using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(KeyName), Name = "IDXKeyValueSynonym_KeyName")]
    [Index(nameof(KsName), Name = "IDXKeyValueSynonym_KsName", IsUnique = true)]
    public partial class KeyValueSynonym
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string KsId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string KeyName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string KsName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
