using Netnr.Core;
using Netnr.SharedFast;
using System.Collections.Generic;

namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class ChatUserBaseVM
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
        /// 设备
        /// </summary>
        public string UserDevice { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string UserSign { get; set; }

        /// <summary>
        /// 到期时间（秒）
        /// </summary>
        public long ExpireDate { get; set; }
    }

    /// <summary>
    /// 用户信息+连接信息
    /// </summary>
    public class ChatUserConnVM : ChatUserBaseVM
    {
        /// <summary>
        /// 连接信息
        /// </summary>
        public Dictionary<string, ChatConnectionVM> Conns { get; set; } = new Dictionary<string, ChatConnectionVM>();
    }

    /// <summary>
    /// 用户信息+授权Token
    /// </summary>
    public class ChatUserTokenVM : ChatUserBaseVM
    {
        /// <summary>
        /// 授权
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 构建token
        /// </summary>
        public void BuildToken()
        {
            var key = GlobalTo.GetValue("TokenManagement:Secret");
            AccessToken = CalcTo.AESEncrypt(this.ToJson(), key);
        }
    }
}
