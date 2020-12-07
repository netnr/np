using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(GistCode), Name = "Gist_GistCode")]
    [Index(nameof(Uid), Name = "Gist_Uid")]
    public partial class Gist
    {
        [Key]
        [StringLength(50)]
        public string GistId { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string GistCode { get; set; }
        [StringLength(50)]
        public string GistFilename { get; set; }
        public string GistContent { get; set; }
        public string GistContentPreview { get; set; }
        public int? GistRow { get; set; }
        [StringLength(50)]
        public string GistLanguage { get; set; }
        [StringLength(50)]
        public string GistTheme { get; set; }
        [StringLength(200)]
        public string GistRemark { get; set; }
        [StringLength(200)]
        public string GistTags { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GistCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GistUpdateTime { get; set; }
        public int? GistOpen { get; set; }
        public int? GistStatus { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
