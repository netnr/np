using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(TagId), Name = "IDXUserWritingTags_TagId")]
    [Index(nameof(TagName), Name = "IDXUserWritingTags_TagName")]
    [Index(nameof(UwId), Name = "IDXUserWritingTags_UwId")]
    public partial class UserWritingTags
    {
        [Key]
        public int UwtId { get; set; }
        public int UwId { get; set; }
        public int? TagId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string TagName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string TagCode { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
