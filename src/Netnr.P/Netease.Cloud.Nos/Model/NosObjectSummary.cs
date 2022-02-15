using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// <see cref="NosObject"/>的摘要信息
    /// </summary>
    public class NosObjectSummary
    {
        /// <summary>
        ///  获取桶名
        /// </summary>
        public string Bucket { get; internal set; }

        /// <summary>
        /// 获取对象名
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取对象的ETag
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        ///获取对象的文件字节数
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        /// 获取对象最后的修改时间
        /// </summary>
        public string LastModified { get; internal set; }

        /// <summary>
        /// 获取对象的存储级别
        /// </summary>
        public string StorageClass { get; internal set; }

        /// <summary>
        /// 获取对象的所有者信息
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        ///构造一个新的<see cref="NosObjectSummary"/>
        /// </summary>
        internal NosObjectSummary()
        { }

        public override string ToString()
        {
            return string.Format(CultureInfo.InstalledUICulture, "[NosObjectSummary Bucket={0}, Key={1}, ETag={2}]", Bucket, Key, ETag);
        }
    }
}
