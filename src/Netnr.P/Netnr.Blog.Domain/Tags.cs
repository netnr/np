using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(TagOwner), nameof(TagPid), nameof(TagOrder), Name = "Index_2")]
    [Index(nameof(TagOwner), nameof(TagPid), nameof(TagOrder), Name = "Tags_TagOwner_TagPid_TagOrder")]
    public partial class Tags
    {
        [Key]
        public int TagId { get; set; }
        [StringLength(50)]
        public string TagName { get; set; }
        [StringLength(20)]
        public string TagCode { get; set; }
        [StringLength(200)]
        public string TagIcon { get; set; }
        public int? TagPid { get; set; }
        public int? TagOwner { get; set; }
        public int? TagOrder { get; set; }
        public int? TagStatus { get; set; }
        public int? TagHot { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
        public int? TagState { get; set; }
    }
}
