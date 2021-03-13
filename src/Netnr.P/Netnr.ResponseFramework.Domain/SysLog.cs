using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    [Index(nameof(LogCreateTime), Name = "IDXSysLog_LogCreateTime")]
    public partial class SysLog
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string LogId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SuName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SuNickname { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string LogAction { get; set; }
        [Column(TypeName = "text")]
        public string LogContent { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string LogUrl { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string LogIp { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string LogArea { get; set; }
        [Column(TypeName = "text")]
        public string LogUserAgent { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string LogBrowserName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string LogSystemName { get; set; }
        public int? LogGroup { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string LogLevel { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogCreateTime { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string LogRemark { get; set; }
    }
}
