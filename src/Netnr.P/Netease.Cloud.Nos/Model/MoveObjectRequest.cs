using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class MoveObjectRequest
    {
        /// <summary>
        /// 获取或设置源Object所在的Bucket的名称
        /// </summary>
        public string SourceBucket { get; set; }

        /// <summary>
        /// 获取或者设置源Object的Key
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// 获取或设置目标Object所在的Bucket的名称。
        /// </summary>
        public string DestinationBucket { get; set; }

        /// <summary>
        /// 获取或设置目标Object的Key。
        /// </summary>
        public string DestinationKey { get; set; }

        public MoveObjectRequest(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey)
        {
            NosUtils.CheckBucketName(destinationBucket);
            NosUtils.CheckObjectName(destinationKey);

            this.SourceBucket = sourceBucket;
            this.SourceKey = sourceKey;
            this.DestinationBucket = destinationBucket;
            this.DestinationKey = destinationKey;
        }

        internal void Populate(IDictionary<string, string> headers)
        {
            var moveHeaderValue = "/" + SourceBucket + "/" + NosUtils.UrlEncodeKey(SourceKey);
            headers.Add(Headers.X_NOS_MOVE_SOURCE, moveHeaderValue);
        }
    }
}
