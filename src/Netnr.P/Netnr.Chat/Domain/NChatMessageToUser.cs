using System;
using System.Collections.Generic;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatMessageToUser
    {
        public string CmuId { get; set; }
        public string CmuPushUserId { get; set; }
        public string CmuPushUserDevice { get; set; }
        public string CmuPushUserSign { get; set; }
        public string CmuPullUserId { get; set; }
        public string CmuContent { get; set; }
        public string CmuPushWhich { get; set; }
        public string CmuPushType { get; set; }
        public DateTime CmuCreateTime { get; set; }
        public int CmuStatus { get; set; }
    }
}
