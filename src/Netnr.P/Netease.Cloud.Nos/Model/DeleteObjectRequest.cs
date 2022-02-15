using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 删除一个对象
    /// </summary>
    public class DeleteObjectRequest
    {
        /// <summary>
        /// 获取或设置<see cref="NosObject"/>所在<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        ///  获取或者设置需要删除的<see cref="NosObject.Key"/>
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 构造一个<see cref="DeleteObjectRequest"/>实例
        /// </summary>
        /// <param name="bucket">桶名</param>
        /// <param name="key">对象名</param>
        public DeleteObjectRequest(string bucket, string key)
        {
            this.Bucket = bucket;
            this.Key = key;
        }
    }
}
