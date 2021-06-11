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
        /// 用户标识
        /// </summary>
        public string UserSign { get; set; }

        /// <summary>
        /// 用户设备，1：PC，2：Mobile
        /// </summary>
        public string UserDevice { get; set; }
    }
}
