using System;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class ListPartsRequest
    {
        private int _maxParts = NosUtils.DEFAULT_MAX_PARTS;

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
        /// 获取或设置分块号的界限，只有更大的分块号会被列出来。
        /// </summary>
        public string PartNumberMarker { get; set; }

        /// <summary>
        /// 获取或设置响应中返回的记录个数。取值范围：0-1000，默认1000。
        /// </summary>
        public int maxParts 
        {
            get { return _maxParts; }
            set { _maxParts = value; }
        }

        public ListPartsRequest(string bucket, string key, string uploadId)
        {
            this.Bucket = bucket;
            this.Key = key;
            this.UploadId = uploadId;
        }
    }
}
