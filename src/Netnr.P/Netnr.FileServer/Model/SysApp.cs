using SQLite;

namespace Netnr.FileServer.Model
{
    /// <summary>
    /// App管理
    /// </summary>
    public class SysApp
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [PrimaryKey, MaxLength(50)]
        public string AppId { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        [Unique, MaxLength(50)]
        public string AppKey { get; set; }

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
        /// 固定Token
        /// </summary>
        public string FixedToken { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Remark { get; set; }
    }
}