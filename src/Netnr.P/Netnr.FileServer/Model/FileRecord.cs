using SQLite;

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
        public string Id { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        [Indexed, MaxLength(50)]
        public string OwnerUser { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [MaxLength(100)]
        public string Type { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [MaxLength(200)]
        public string Path { get; set; }

        /// <summary>
        /// 文件Hash，SHA1
        /// </summary>
        [MaxLength(100)]
        public string Hash { get; set; }

        /// <summary>
        /// 文件大小，单位B
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Remark { get; set; }

    }
}