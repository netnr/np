using System;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Authentication
{
    /// <summary>
    /// 默认鉴权类
    /// </summary>
    public class DefaultCredentials : ICredentials
    {
        /// <summary>
        /// 获取或设置NOS的访问ID
        /// </summary>
        public string AccessKeyId { get; private set; }

        /// <summary>
        /// 获取或设置NOS的访问密钥
        /// </summary>
        public string AccessKeySecret { get; private set; }

        /// <summary>
        /// 构造一个<see cref="DefaultCredentials"/>实例
        /// </summary>
        /// <param name="accessKeyId">NOS的访问ID</param>
        /// <param name="accessKeySecret">NOS的访问密钥</param>
        public DefaultCredentials(string accessKeyId, string accessKeySecret)
        {
            NosUtils.CheckCredentials(accessKeyId, accessKeySecret);

            this.AccessKeyId = accessKeyId.Trim();
            this.AccessKeySecret = accessKeySecret.Trim();
        }
    }
}
