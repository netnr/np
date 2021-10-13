using SQLite;

namespace Netnr.FileServer.Model
{
    /// <summary>
    /// 生成Token
    /// </summary>
    public class SysToken
    {
        /// <summary>
        /// 所属用户，唯一，文件夹名
        /// </summary>
        [Unique, MaxLength(50)]
        public string Owner { get; set; }

        /// <summary>
        /// 生成的Token
        /// </summary>
        [Unique, MaxLength(200)]
        public string Token { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime TokenExpireTime { get; set; }
    }
}
