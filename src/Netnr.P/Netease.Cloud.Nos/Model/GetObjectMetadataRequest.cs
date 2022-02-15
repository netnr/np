using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取对象的<see cref="ObjectMetadata"/>
    /// </summary>
    public class GetObjectMetadataRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置需要获取元信息的<see cref="NosObject"/>的Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 获取或设置指定时间，只有当指定时间之后做过修改操作才返回这个对象
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }

        /// <summary>
        /// <see cref="GetObjectMetadataRequest"/>的构造函数
        /// </summary>
        /// <param name="bucket"><see cref="OssObject"/>所在桶名</param>
        /// <param name="key">需要获取元信息的对象名称</param>
        public GetObjectMetadataRequest(string bucket, string key)
        {
            this.Bucket = bucket;
            this.Key = key;
        }

        internal void Populate(IDictionary<string, string> headers)
        {

            if (IfModifiedSince != null)
                headers.Add(Headers.GET_OBJECT_IF_MODIFIED_SINCE, DateUtils.FormatRfc822Date(IfModifiedSince.Value));
        }
    }
}
