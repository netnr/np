using System.Collections.Generic;

namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 通讯组
    /// </summary>
    public class ChatGroupVM
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 组归类
        /// </summary>
        public string GroupClassify { get; set; }

        /// <summary>
        /// 组用户
        /// </summary>
        public Dictionary<string, ChatUserVM> GroupUser { get; set; } = new Dictionary<string, ChatUserVM>();
    }
}
