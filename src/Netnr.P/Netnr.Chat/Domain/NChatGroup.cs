using System;
using System.Collections.Generic;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatGroup
    {
        public string CgId { get; set; }
        public string CgName { get; set; }
        public string CgOwnerId { get; set; }
        public string CcId { get; set; }
        public DateTime CgCreateTime { get; set; }
        public int CgStatus { get; set; }
    }
}
