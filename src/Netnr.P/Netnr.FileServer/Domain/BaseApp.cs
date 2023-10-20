namespace Netnr.FileServer.Domain
{
    /// <summary>
    /// 应用
    /// </summary>
    public class BaseApp
    {
        /// <summary>
        /// 应用ID，唯一
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 密钥，唯一
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 所属用户，唯一，文件夹名
        /// </summary>
        public string AppOwner { get; set; }

        /// <summary>
        /// 固定Token
        /// </summary>
        public string AppFixedToken { get; set; }

        /// <summary>
        /// 生成的Token，唯一
        /// </summary>
        public string AppToken { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime AppTokenExpireTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string AppRemark { get; set; }
    }
}