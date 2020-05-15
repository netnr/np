using System;

namespace Netnr.TencentAI.Model
{
    /// <summary>
    /// 必填特性
    /// </summary>
    public class Required : Attribute
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
    }
}