namespace Netnr.FileServer.Domain
{
    /// <summary>
    /// 文件
    /// </summary>
    public class BaseFile
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public string OwnerUser { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件Hash，SHA1
        /// </summary>
        public string FileHash { get; set; }

        /// <summary>
        /// 文件大小，单位B
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 备注
        /// </summary>
        public string FileRemark { get; set; }

    }
}