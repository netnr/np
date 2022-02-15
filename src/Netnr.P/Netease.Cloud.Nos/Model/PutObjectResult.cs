using System;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 表示上传对象的返回结果
    /// </summary>
    public class PutObjectResult
    {
        /// <summary>
        /// 获取对象的ETag
        /// </summary>
        public string ETag { get; internal set; }

        internal PutObjectResult()
        { }
    }
}
