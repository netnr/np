using System;
using System.Collections.Generic;
using System.IO;

namespace Netease.Cloud.NOS.Service
{
    public class ServiceMessage
    {
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 获取HTTP请求头或响应头信息
        /// </summary>
        public virtual IDictionary<string, string> Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// 获取或设置流
        /// </summary>
        public virtual Stream Content { get; set; }
    }
}
