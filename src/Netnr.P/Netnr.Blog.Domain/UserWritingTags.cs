using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(TagId), Name = "UserWritingTags_TagsId")]
    [Index(nameof(TagName), Name = "UserWritingTags_TagsName")]
    public partial class UserWritingTags
    {
        [Key]
        public int UwtId { get; set; }
        public int UwId { get; set; }
        public int? TagId { get; set; }
        [StringLength(50)]
        public string TagName { get; set; }
        [StringLength(20)]
        public string TagCode { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
