using SQLite;
using System;

namespace Netnr.FileServer.Model
{
    /// <summary>
    /// token管理
    /// </summary>
    public class SysKey
    {
        /// <summary>
        /// AppId
        /// </summary>
        [PrimaryKey, MaxLength(50)]
        public string SkAppId { get; set; }

        /// <summary>
        /// SkAppKey    密钥，取Guid的MD5值
        /// </summary>
        [Unique, MaxLength(50)]
        public string SkAppKey { get; set; }

        /// <summary>
        /// 所属用户，唯一，文件夹名
        /// </summary>
        [Unique, MaxLength(50)]
        public string SkOwner { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? SkCreateTime { get; set; }

        /// <summary>
        /// 生成的Token
        /// </summary>
        [Unique, MaxLength(100)]
        public string SkToken { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime? SkTokenExpireTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string SkRemark { get; set; }
    }
}