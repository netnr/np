using System;

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysLog
    {
        public string LogId { get; set; }
        public string SuName { get; set; }
        public string SuNickname { get; set; }
        public string LogAction { get; set; }
        public string LogContent { get; set; }
        public string LogUrl { get; set; }
        public string LogIp { get; set; }
        public string LogArea { get; set; }
        public string LogUserAgent { get; set; }
        public string LogBrowserName { get; set; }
        public string LogSystemName { get; set; }
        public int? LogGroup { get; set; }
        public string LogLevel { get; set; }
        public DateTime? LogCreateTime { get; set; }
        public string LogRemark { get; set; }
    }
}
