using System;
using System.Collections.Generic;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatBuddy
    {
        public string CbId { get; set; }
        public string CuUserId { get; set; }
        public string CbUserId { get; set; }
        public string CcId { get; set; }
        public DateTime CbCreateTime { get; set; }
    }
}
