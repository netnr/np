using System;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class ListObjectsRequest
    {
        private string _prefix;
        private string _marker;
        private int _maxKeys = NosUtils.DEFAULT_RETURNED_KEYS;
        private string _delimiter;

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或设置返回对象的前缀开头
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        /// <summary>
        /// 获取或设置字典序的起始标记
        /// </summary>
        public string Marker
        {
            get { return _marker; }
            set { _marker = value; }
        }

        /// <summary>
        /// 获取或设置限定返回对象的最大数量
        /// 默认是1000
        /// </summary>
        public int MaxKeys
        {
            get { return _maxKeys; }
            set { _maxKeys = value; }
        }

        /// <summary>
        /// 获取或设置用于对返回结果进行分组的值
        /// </summary>
        public string Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        /// <summary>
        /// 构造一个<see cref="ListObjectsRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        public ListObjectsRequest(string bucket)
        {
            Bucket = bucket;
        }
    }
}
