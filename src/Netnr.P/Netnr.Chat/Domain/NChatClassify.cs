using System;
using System.Collections.Generic;

#nullable disable

namespace Netnr.Chat.Domain
{
    public partial class NChatClassify
    {
        public string CcId { get; set; }
        public string CuUserId { get; set; }
        public string CcName { get; set; }
        public int CcType { get; set; }
        public int CcOrder { get; set; }
    }
}
