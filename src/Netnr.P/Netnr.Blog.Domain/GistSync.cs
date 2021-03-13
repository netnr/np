using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class GistSync
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string GistCode { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GistFilename { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GsGitHubId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GsGitHubTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string GsGiteeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GsGiteeTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
