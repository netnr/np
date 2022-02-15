using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class CopyObjectRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingEtagConstraints = new List<string>();

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
        /// 获取或设置目标Object的Key
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// 获取或设置存储级别
        /// </summary>
        public string StorageClass { get; set; }      

        public CopyObjectRequest(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey)
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
            var copyHeaderValue = "/" + SourceBucket + "/" + NosUtils.UrlEncodeKey(SourceKey);
            headers.Add(Headers.X_NOS_COPY_SOURCE, copyHeaderValue);

            headers.Remove(Headers.CONTENT_LENGTH);
        }
    }
}
