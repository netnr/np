using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 块的PartNumber和ETag信息
    /// </summary>
    public class PartETag
    {
        /// <summary>
        /// 获取或设置分块的标识号
        /// </summary>
        public int PartNumber { get; set; }

        /// <summary>
        /// 获取或设置分块的ETag
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// 构造一个新的<see cref="PartETag"/>实例
        /// </summary>
        /// <param name="partNumber">分块标识号</param>
        /// <param name="eTag">分块的ETag值</param>
        public PartETag(int partNumber, string eTag)
        {
            this.PartNumber = partNumber;
            this.ETag = eTag;
        }
    }
}
