using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 取消分块上传的返回结果
    /// </summary>
    public class UploadPartResult
    {
        /// <summary>
        /// 获取对象的ETag
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// 获取分块标识
        /// </summary>
        public int PartNumber { get; internal set; }

        /// <summary>
        /// 获取包含分块标识号和ETag值得<see cref="PartEtag"/>对象
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag); }
        }

        internal UploadPartResult()
        { }
    }
}
