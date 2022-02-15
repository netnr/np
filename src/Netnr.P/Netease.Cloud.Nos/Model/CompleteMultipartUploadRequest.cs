using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 完成分块上传
    /// </summary>
    public class CompleteMultipartUploadRequest
    {
        private readonly IList<PartETag> _partETags = new List<PartETag>();

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置<see cref="OssObject" />的值。
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 获取或设置数据上传标识号
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// 获取或设置标识分块上传结果的<see cref="PartETag" />对象列表。
        /// </summary>
        public IList<PartETag> PartETags
        {
            get { return _partETags; }
        }

        public string ObjectMD5 { get; set; }

        /// <summary>
        /// 构造一个<see cref="CompleteMultipartUploadRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        /// <param name="uploadId">本次需要完成的上传标识号</param>
        public CompleteMultipartUploadRequest(string bucket, string key, string uploadId)
        {
            this.Bucket = bucket;
            this.Key = key;
            this.UploadId = uploadId;
        }

        internal void Populate(IDictionary<string, string> headers)
        {

            if (ObjectMD5 != null)
                headers.Add(Headers.X_NOS_OBJECT_MD5, ObjectMD5);
        }
    }
}
