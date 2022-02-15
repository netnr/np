using System;

namespace Netease.Cloud.NOS.Authentication
{
    public interface ICredentials
    {
        // <summary>
        /// NOS的访问ID
        /// </summary>
        string AccessKeyId { get; }

        /// <summary>
        /// NOS的访问密钥
        /// </summary>
        string AccessKeySecret { get; }
    }
}
