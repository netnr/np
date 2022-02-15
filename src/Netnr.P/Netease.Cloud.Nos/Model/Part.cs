using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取分块上传中块的数据信息
    /// </summary>
    public class Part
    {
        /// <summary>
        /// 获取识别特定part的一串数字
        /// </summary>
        public int PartNumber { get; internal set; }

        /// <summary>
        /// 获取该part上传的时间
        /// </summary>
        public string LastModified { get; internal set; }

        /// <summary>
        /// 获取分块内容的ETag
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// 已上传的 part数据的大小
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        /// 获取包含Part标识号码和ETag值的<see cref="PartETag" />对象
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag); }
        }

        internal Part()
        { }

        public override string ToString()
        {
            return string.Format(CultureInfo.InstalledUICulture, "[Part PartNumber={0}, ETag={1}, LastModified={2}, Size={3}]", PartNumber, ETag, LastModified, Size);
        }
    }
}
