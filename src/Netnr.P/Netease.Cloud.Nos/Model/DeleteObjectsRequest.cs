using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class DeleteObjectsRequest
    {
        private readonly IList<string> _keys = new List<string>();

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// 获取或者设置需要删除的key列表
        /// </summary>
        public IList<string> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        /// 获取或设置删除模式
        /// </summary>
        public bool Quiet { get; private set; }

        /// <summary>
        ///  使用静默方式的<see cref="DeleteObjectsRequest"/>构造函数。
        /// </summary>
        /// <param name="bucket"><see cref="OssObject"/>所在桶名</param>
        /// <param name="keys">需删除的对象列表</param>
        public DeleteObjectsRequest(string bucket, IList<string> keys)
            : this(bucket, keys, true)
        { }

        /// <summary>
        /// <see cref="DeleteObjectsRequest"/>的构造函数
        /// </summary>
        /// <param name="bucket"><see cref="OssObject"/>所在桶名</param>
        /// <param name="keys">需删除的对象列表</param>
        /// <param name="quiet">删除模式（true表示静默模式，false表示详细模式）</param>
        public DeleteObjectsRequest(string bucket, IList<string> keys, bool quiet)
        {
            if (keys == null)
                throw new ArgumentException("The list of Keys to be deleted should not be nall.");
            if (keys.Count <= 0)
                throw new ArgumentException("No any keys specified.");
            if (keys.Count > NosUtils.OBJECTS_UPPER_LIMIT)
                throw new ArgumentException("Count of Objects to be deleted exceeds upper limit.");

            this.Bucket = bucket;
            foreach (var key in keys)
            {
                NosUtils.CheckObjectName(key);
                Keys.Add(key);
            }
            this.Quiet = quiet;
        }
    }
}
