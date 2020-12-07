using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysLog
    {
        [Key]
        [StringLength(50)]
        public string LogId { get; set; }
        [StringLength(50)]
        public string SuName { get; set; }
        [StringLength(50)]
        public string SuNickname { get; set; }
        [StringLength(50)]
        public string LogAction { get; set; }
        [StringLength(200)]
        public string LogContent { get; set; }
        [StringLength(500)]
        public string LogUrl { get; set; }
        [StringLength(50)]
        public string LogIp { get; set; }
        [StringLength(50)]
        public string LogArea { get; set; }
        [StringLength(500)]
        public string LogUserAgent { get; set; }
        [StringLength(50)]
        public string LogBrowserName { get; set; }
        [StringLength(50)]
        public string LogSystemName { get; set; }
        public int? LogGroup { get; set; }
        [StringLength(10)]
        public string LogLevel { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogCreateTime { get; set; }
        [StringLength(200)]
        public string LogRemark { get; set; }
    }
}
