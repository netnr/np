using System.Collections.Generic;

namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 通讯用户
    /// </summary>
    public class ChatUserVM
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string UserPhoto { get; set; }

        /// <summary>
        /// 连接信息
        /// </summary>
        public Dictionary<string, ChatConnectionVM> Conns { get; set; } = new Dictionary<string, ChatConnectionVM>();
    }
}
