using System;
using System.Collections.Generic;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 取消分块上传并删除已上传的分块
    /// </summary>
    public class AbortMultipartUploadRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置<see cref="OssObject" />的值。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 获取或设置数据上传标识号
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// 构造一个<see cref="AbortMultipartUploadRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="uploadId">数据上传标识号</param>
        public AbortMultipartUploadRequest(string bucket, string key, string uploadId)
        {
            this.Bucket = bucket;
            this.Key = key;
            this.UploadId = uploadId;
        }
    }
}
