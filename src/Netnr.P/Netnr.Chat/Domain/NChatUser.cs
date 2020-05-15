using System;

namespace Netnr.Chat.Domain
{
    public partial class NChatUser
    {
        public string CuUserId { get; set; }
        public string CuUserName { get; set; }
        public string CuUserNickname { get; set; }
        public string CuPassword { get; set; }
        public string CuUserPhoto { get; set; }
        public DateTime CuCreateTime { get; set; }
        public int CuStatus { get; set; }
    }
}
