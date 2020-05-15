using System;

namespace Netnr.Chat.Domain
{
    public partial class NChatMessageGroupPull
    {
        public string GpId { get; set; }
        public string CuUserId { get; set; }
        public string CgGroupId { get; set; }
        public string CmgId { get; set; }
        public DateTime GpUpdateTime { get; set; }
    }
}
