using System;

namespace Netnr.Chat.Domain
{
    public partial class NChatGroupMember
    {
        public string CgmId { get; set; }
        public string CgId { get; set; }
        public string CuUserId { get; set; }
        public string CuMark { get; set; }
        public DateTime CgmCreateTime { get; set; }
        public int CgmStatus { get; set; }
    }
}
