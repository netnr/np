namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 连接信息
    /// </summary>
    public class ChatConnectionVM
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnId { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 连接标识
        /// </summary>
        public string ConnSign { get; set; }

        /// <summary>
        /// 设备，1：PC，2：Mobile
        /// </summary>
        public string UserDevice { get; set; }
    }
}
