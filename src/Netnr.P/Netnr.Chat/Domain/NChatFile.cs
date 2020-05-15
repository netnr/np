using System;

namespace Netnr.Chat.Domain
{
    public partial class NChatFile
    {
        public string CfId { get; set; }
        public string CuUserId { get; set; }
        public string CfFileName { get; set; }
        public string CfFullPath { get; set; }
        public string CfType { get; set; }
        public string CfExt { get; set; }
        public long CfSize { get; set; }
        public DateTime CfCreateTime { get; set; }
        public int CfStatus { get; set; }
    }
}
