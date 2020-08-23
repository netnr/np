using SQLite;
using System;

namespace Netnr.FileServer.Model
{
    /// <summary>
    /// 文件记录
    /// </summary>
    public class FileRecord
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, MaxLength(50)]
        public string FrId { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        [Indexed, MaxLength(50)]
        public string FrOwnerUser { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [MaxLength(100)]
        public string FrType { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(500)]
        public string FrName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [MaxLength(200)]
        public string FrPath { get; set; }

        /// <summary>
        /// 文件Hash，SHA1
        /// </summary>
        [MaxLength(100)]
        public string FrHash { get; set; }

        /// <summary>
        /// 文件大小，单位B
        /// </summary>
        public long FrSize { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FrCreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string FrRemark { get; set; }

    }
}