using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取对象
    /// </summary>
    public class GetObjectRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置需要下载的<see cref="NosObject"/>的Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 下载指定的数据块，Range Header参考RFC2616。
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 获取或设置“If-Modified-Since”参数。
        /// 只有当指定时间之后做过修改操作才返回这个对象
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }

        /// <summary>
        /// 如果传入期望的ETag和<see cref="NosObject" />的ETag匹配，则返回这个对象
        /// </summary>
        public IList<string> MatchingETagConstraints
        {
            get { return _matchingETagConstraints; }
        }

        /// <summary>
        /// <see cref="GetObjectRequest"/>的构造函数
        /// </summary>
        /// <param name="bucket"><see cref="NosObject"/>所在桶名</param>
        /// <param name="key">需要下载的对象名称</param>
        public GetObjectRequest(string bucket, string key)
        {
            this.Bucket = bucket;
            this.Key = key;
        }

        internal void Populate(IDictionary<string, string> headers)
        {
            if (Range != null)
                headers.Add(Headers.RANGE, Range);

            if (IfModifiedSince != null)
                headers.Add(Headers.GET_OBJECT_IF_MODIFIED_SINCE, DateUtils.FormatRfc822Date(IfModifiedSince.Value));
        }
    }
}
