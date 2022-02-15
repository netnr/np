using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 完成分块上传的请求结果
    /// </summary>
    public class CompleteMultipartUploadResult
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
        /// 获取新创建的对象的URL
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// 获取新创建的对象的ETag
        /// </summary>
        public string ETag { get; internal set; }

        public CompleteMultipartUploadResult()
        { }
    }
}
