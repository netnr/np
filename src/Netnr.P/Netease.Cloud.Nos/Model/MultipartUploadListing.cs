using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取列出所有执行中的分块上传事件的请求结果
    /// </summary>
    public class MultipartUploadListing
    {
        private readonly IList<MultipartUpload> _multipartUploads = new List<MultipartUpload>();
        private readonly IList<string> _commonPrefixes = new List<string>();

        /// <summary>
        /// 获取对象所在的名称
        /// </summary>
        public string Bucket { get; internal set; }

        public string KeyMarker { get; internal set; }

        /// <summary>
        /// 作为后续查询的key-marker 类型
        /// </summary>
        public string NextKeyMarker { get; internal set; }

        /// <summary>
        /// 获取是否截断，如果因为设置了limit导致不是所有的数据集都返回了，则该值设置为true 
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// 获取请求的对象的Key的前缀 
        /// </summary>
        public string Prefix { get; internal set; }

        public string Delimiter { get; internal set; }

        /// <summary>
        /// 最多返回max-uploads条记录
        /// </summary>
        public int MaxUploads { get; internal set; }

        /// <summary>
        /// 获取所有的Multipart Upload事件
        /// </summary>
        public IEnumerable<MultipartUpload> MultipartUploads
        {
            get { return _multipartUploads; }
        }

        /// <summary>
        /// 获取返回结果中的CommonPrefixes部分。
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        /// <summary>
        /// 构造一个<see cref="MultipartUploadListing" />实例
        /// </summary>
        /// <param name="bucket">对象所在的桶名</param>
        public MultipartUploadListing(string bucket)
        {
            Bucket = bucket;
        }

        /// <summary>
        /// 增加<see cref="MultipartUpload"/>事件
        /// </summary>
        /// <param name="multipartUpload">事件信息</param>
        internal void AddMultipartUpload(MultipartUpload multipartUpload)
        {
            _multipartUploads.Add(multipartUpload);
        }

        /// <summary>
        /// 增加公共前缀
        /// </summary>
        /// <param name="prefix">需要增加的前缀字符串</param>
        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }
    }
}
