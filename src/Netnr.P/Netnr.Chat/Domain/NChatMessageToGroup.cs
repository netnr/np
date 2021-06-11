using System;
using System.Collections.Generic;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatMessageToGroup
    {
        public string CmgId { get; set; }
        public string CmgPushUserId { get; set; }
        public string CmgPushUserDevice { get; set; }
        public string CmgPushUserSign { get; set; }
        public string CmgPullGroupId { get; set; }
        public string CmgContent { get; set; }
        public string CmgPushWhich { get; set; }
        public string CmgPushType { get; set; }
        public DateTime CmgCreateTime { get; set; }
    }
}
