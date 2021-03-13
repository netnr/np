using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(TagName), Name = "IDXTags_TagName", IsUnique = true)]
    [Index(nameof(TagOwner), nameof(TagPid), nameof(TagOrder), Name = "IDXTags_TagOwner_TagPid_TagOrder")]
    public partial class Tags
    {
        [Key]
        public int TagId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string TagName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string TagCode { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string TagIcon { get; set; }
        public int? TagPid { get; set; }
        public int? TagOwner { get; set; }
        public int? TagOrder { get; set; }
        public int? TagStatus { get; set; }
        public int? TagHot { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
        public int? TagState { get; set; }
    }
}
