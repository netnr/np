using System;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 列出所有执行中的分块上传事件
    /// </summary>
    public class ListMultipartUploadsRequest
    {
        private int _maxUploads = NosUtils.DEFAULT_MAX_UPLOADS;

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置NOS返回罗列结果内Multipart Upload信息的数目。
        /// 如果不设定，默认1000
        /// 不能大于1000
        /// </summary>
        public int MaxUploads 
        {
            get { return _maxUploads; }
            set { _maxUploads = value; }
        }

        /// <summary>
        /// 获取或设置keyMarker,只有大于该keyMarker的才会被列出。
        /// </summary>
        public string KeyMarker { get; set; }

        /// <summary>
        /// 获取或设置返回对象的前缀
        /// </summary>
        public string Prefix { get; set; }

        public ListMultipartUploadsRequest(string bucket)
        {
            this.Bucket = bucket;

        }
    }
}
