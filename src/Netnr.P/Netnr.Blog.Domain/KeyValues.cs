using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(KeyName), Name = "IDXKeyValues_KeyName", IsUnique = true)]
    public partial class KeyValues
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string KeyId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string KeyName { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue1 { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue2 { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue3 { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue4 { get; set; }
        [Column(TypeName = "longtext")]
        public string KeyValue5 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string KeyType { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string KeyRemark { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
