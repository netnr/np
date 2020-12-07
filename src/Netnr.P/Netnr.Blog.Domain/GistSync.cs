using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class GistSync
    {
        [Key]
        [StringLength(50)]
        public string GistCode { get; set; }
        [StringLength(50)]
        public string GistFilename { get; set; }
        public int? Uid { get; set; }
        [StringLength(50)]
        public string GsGitHubId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GsGitHubTime { get; set; }
        [StringLength(50)]
        public string GsGiteeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GsGiteeTime { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
