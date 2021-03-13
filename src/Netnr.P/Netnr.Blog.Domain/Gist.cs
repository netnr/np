using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(GistCode), Name = "IDXGist_GistCode")]
    [Index(nameof(Uid), Name = "IDXGist_Uid")]
    public partial class Gist
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string GistId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GistCode { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GistFilename { get; set; }
        [Column(TypeName = "longtext")]
        public string GistContent { get; set; }
        [Column(TypeName = "longtext")]
        public string GistContentPreview { get; set; }
        public int? GistRow { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GistLanguage { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GistTheme { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GistRemark { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string GistTags { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GistCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GistUpdateTime { get; set; }
        public int? GistOpen { get; set; }
        public int? GistStatus { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
