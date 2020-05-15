using System;
using System.Collections.Generic;

namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 推送消息
    /// </summary>
    public class ChatMessageVM
    {
        /// <summary>
        /// 发送者用户ID
        /// </summary>
        public string CmFromId { get; set; }

        /// <summary>
        /// 发送者连接信息
        /// </summary>
        public ChatConnectionVM CmFromConn { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string CmId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CmTime { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public object CmContent { get; set; }

        /// <summary>
        /// 推送哪种
        /// </summary>
        public string CmWhich { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string CmType { get; set; }

        /// <summary>
        /// 接收者用户/组
        /// </summary>
        public List<string> CmToIds { get; set; }

        /// <summary>
        /// 消息状态（-1：撤回，1：待推送，2：已推送，3：已接收，4：已读）
        /// </summary>
        public int CmStatus { get; set; }
    }
}
