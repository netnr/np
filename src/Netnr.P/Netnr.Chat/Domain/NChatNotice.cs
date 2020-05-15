using System;

namespace Netnr.Chat.Domain
{
    public partial class NChatNotice
    {
        public string CnId { get; set; }
        public string CuUserId { get; set; }
        public string CnFromId { get; set; }
        public string CnNotice1 { get; set; }
        public string CnNotice2 { get; set; }
        public int CnType { get; set; }
        public int? CnResult { get; set; }
        public string CnReason { get; set; }
        public DateTime CnCreateTime { get; set; }
        public int CnStatus { get; set; }
    }
}
