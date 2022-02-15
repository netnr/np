using System;
using System.Collections.Generic;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 初始化大对象分块上传
    /// </summary>
    public class InitiateMultipartUploadRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置<see cref="NosObject" />的值。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 获取或设置<see cref="ObjectMetadata" />
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// 获取或设置对象的存储级别
        /// </summary>
        public string StorageClass { get; set; }

        /// <summary>
        /// 构造一个<see cref="InitiateMultipartUploadRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="objectMetadata">对象元数据</param>
        public InitiateMultipartUploadRequest(string bucket, string key, ObjectMetadata objectMetadata)
        {
            this.Bucket = bucket;
            this.Key = key;
            this.ObjectMetadata = objectMetadata;
        }

        public InitiateMultipartUploadRequest(string bucket, string key)
            : this(bucket, key, null)
        { }
    }
}
