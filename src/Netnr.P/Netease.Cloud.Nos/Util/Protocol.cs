using System;

namespace Netease.Cloud.NOS.Util
{
    /// <summary>
    /// 通讯协议，默认是HTTP
    /// </summary>
    public enum Protocol
    {
        /// <summary>
        /// 超文本传输协议
        /// </summary>
        [StringValue("http")]
        Http = 0,

        /// <summary>
        /// 超文本安全传输协议
        /// </summary>
        [StringValue("https")]
        Https
    }
}
