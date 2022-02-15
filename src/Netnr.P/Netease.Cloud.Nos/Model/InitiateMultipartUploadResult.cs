using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 表示初始化分块上传的结果
    /// </summary>
    public class InitiateMultipartUploadResult
    {
        /// <summary>
        /// 获取桶名
        /// </summary>
        public string Bucket { get; internal set; }

        /// <summary>
        /// 获取对象名
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取上传的标识符
        /// </summary>
        public string UploadId { get; internal set; }

        internal InitiateMultipartUploadResult()
        { }
    }
}
