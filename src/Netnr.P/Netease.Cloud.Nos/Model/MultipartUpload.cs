using System;
using System.Globalization;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取所有执行中的分块上传事件的信息
    /// </summary>
    public class MultipartUpload
    {
        /// <summary>
        /// 获取对象的名称
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取上传ID
        /// </summary>
        public string UploadId { get; internal set; }

        /// <summary>
        /// 获取对象存储级别
        /// </summary>
        public string StorageClass { get; internal set; }

        /// <summary>
        /// 获取该分块上传操作被初始化的时间
        /// </summary>
        public DateTime Initiated { get; internal set; }

        /// <summary>
        /// 获取桶拥有者的信息
        /// </summary>
        public Owner owner { get; internal set; }

        internal MultipartUpload()
        { }

        public override string ToString()
        {
            return string.Format(CultureInfo.InstalledUICulture, "[MultipartUpload key={0}, UploadId={1}]", Key, UploadId);
        }
    }
}
