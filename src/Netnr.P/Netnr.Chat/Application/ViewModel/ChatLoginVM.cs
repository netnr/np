namespace Netnr.Chat.Application.ViewModel
{
    /// <summary>
    /// 登录授权
    /// </summary>
    public class ChatLoginVM
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string Sign { get; set; }
    }
}
