using System;
using System.Text;
using System.Collections.Generic;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 包含获取对象列表的信息
    /// </summary>
    public class ObjectListing
    {
        private readonly List<NosObjectSummary> _objectSummaries = new List<NosObjectSummary>();
        private readonly List<string> _commonPrefixes = new List<string>();

        /// <summary>
        /// 获取对象所在的桶名
        /// </summary>
        public string Bucket { get; internal set; }

        /// <summary>
        /// 获取请求的对象的Key的前缀 
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// 获取请求的对象个数限制
        /// </summary>
        public int MaxKeys { get; internal set; }

        /// <summary>
        /// 获取下一次分页的起点
        /// </summary>
        public string NextMarker { get; internal set; }

        /// <summary>
        /// 获取是否截断，如果因为设置了limit导致不是所有的数据集都返回，则该值设置为true 类型
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// 获取分界符类型
        /// </summary>
        public string Delimiter { get; internal set; }

        /// <summary>
        /// 获取列表的起始位置
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// 获取满足查询条件的<see cref="OssObjectSummary" />
        /// </summary>
        public IEnumerable<NosObjectSummary> ObjectSummarise
        {
            get { return _objectSummaries; }
        }

        /// <summary>
        /// 获取返回结果中的CommonPrefixes部分
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        internal ObjectListing(string bucket)
        {
            this.Bucket = bucket;
        }

        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }

        internal void AddObjectSummary(NosObjectSummary summary)
        {
            _objectSummaries.Add(summary);
        }

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ObjectListing bucketName=" + this.Bucket);
            builder.Append(", delimiter=" + this.Delimiter);
            builder.Append(", maxKeys=" + this.MaxKeys);
            builder.Append(", prefix=" + this.Prefix);
            builder.Append(", marker=" + this.Marker);
            builder.Append(", nextMarker=" + this.NextMarker);
            builder.Append(", isTruncated=" + this.IsTruncated + "]");

            foreach (NosObjectSummary objectSummary in this.ObjectSummarise)
                builder.Append("\nObject:\n" + objectSummary.ToString());
            foreach (String s in this.CommonPrefixes)
                builder.Append("\nCommonPrefix:\n" + s);

            return builder.ToString();
        }
    }
}
