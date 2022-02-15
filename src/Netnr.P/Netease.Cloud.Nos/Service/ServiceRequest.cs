using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    public class ServiceRequest : ServiceMessage, IDisposable
    {
        private bool _disposed;

        private readonly IDictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>
        /// 获取或设置NOS访问地址
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// 获取或设置访问的资源路径
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// 获取或设置发送HTTP请求方法
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// 获取查询参数
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// 获取请求是否可以重复
        /// </summary>
        public bool IsRequestRepeatable
        {
            get { return Content == null || Content.CanSeek; }
        }

        /// <summary>
        /// 构造请求URI
        /// </summary>
        /// <returns>请求URI</returns>
        public string CreateRequestUri()
        {
            const string delimiter = "/";
            var uri = Endpoint.ToString();
            if (!uri.EndsWith(delimiter) &&
                (ResourcePath == null || !ResourcePath.StartsWith(delimiter)))
            {
                uri += delimiter;
            }

            if (ResourcePath != null)
                uri += ResourcePath;

            if (IsParameterInUri())
            {
                var paramString = HttpUtils.ConbineQueryString(_parameters);
                if (!string.IsNullOrEmpty(paramString))
                    uri += "?" + paramString;
            }

            return uri;
        }

        /// <summary>
        /// 构造请求Content
        /// </summary>
        /// <returns>请求Content</returns>
        public Stream CreateRequestContent()
        {
            if (!IsParameterInUri())
            {
                var paramString = HttpUtils.ConbineQueryString(_parameters);
                if (!string.IsNullOrEmpty(paramString))
                {
                    var buffer = Encoding.GetEncoding("utf-8").GetBytes(paramString);
                    Stream content = new MemoryStream();
                    content.Write(buffer, 0, buffer.Length);
                    content.Flush();
                    content.Seek(0, SeekOrigin.Begin);
                    return content;
                }
            }
            return Content;
        }

        /// <summary>
        /// 是否有查询参数在URI中
        /// </summary>
        /// <returns>布尔值</returns>
        private bool IsParameterInUri()
        {
            var requestHasPayload = Content != null;
            var requestIsPost = Method == HttpMethod.POST;
            bool IsHasParamter = !requestIsPost || requestHasPayload;
            return IsHasParamter;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (Content != null)
                {
                    Content.Close();
                    Content = null;
                }
                _disposed = true;
            }
        }
    }
}
