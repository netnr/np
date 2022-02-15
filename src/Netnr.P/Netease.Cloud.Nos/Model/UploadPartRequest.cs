using System;
using System.Collections.Generic;
using System.IO;

namespace Netease.Cloud.NOS
{
    public class UploadPartRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置<see cref="NosObject" />的值。
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 获取或设置数据上传标识号
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// 获取或设置分块编码号
        /// 范围是1-10,000
        /// </summary>
        public int? PartNumber { get; set; }

        /// <summary>
        /// 获取或设置分块的字节数
        /// 除最后一个Part外，其他Part最小为5MB。
        /// </summary>
        public long PartSize { get; set; }

        /// <summary>
        /// 获取或设置对象内容
        /// </summary>
        public Stream Content { get; set; }

        public UploadPartRequest(string bucket, string key, string uploadId)
        {
            this.Bucket = bucket;
            this.Key = key;
            this.UploadId = uploadId;
        }
    }
}
