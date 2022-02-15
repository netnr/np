using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 对象元数据
    /// </summary>
    public class ObjectMetadata
    {
        //用户自定义元数据
        private IDictionary<string, string> _userMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //所有其他不是用户定义的元数据
        private IDictionary<string, object> _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public const string AES_256_SERVER_SIDE_ENCRYPTION = "AES256";

        /// <summary>
        /// 获取用户自定义元数据
        /// </summary>
        public IDictionary<string, string> UserMetadata
        {
            get { return _userMetadata; }
            set { _userMetadata = value; }
        }

        /// <summary>
        ///  获取Content-Length请求头,表示内容大小
        /// </summary>
        public long ContentLength
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_LENGTH)
                    ? (long)_metadata[Headers.CONTENT_LENGTH] : 0;
            }
            set
            {
                _metadata[Headers.CONTENT_LENGTH] = value;
            }
        }

        /// <summary>
        /// 获取或设置Content-Type请求头，表示内容的类型
        /// </summary>
        public string ContentType
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_TYPE)
                    ? _metadata[Headers.CONTENT_TYPE] as string : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _metadata[Headers.CONTENT_TYPE] = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置Content-MD5,表示MD5摘要
        /// </summary>
        public string ContentMD5
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_MD5)
                    ? _metadata[Headers.CONTENT_MD5] as string : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _metadata[Headers.CONTENT_MD5] = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置Content-Encoding,表示内容编码
        /// </summary>
        public string ContentEncoding
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_ENCODING)
                    ? _metadata[Headers.CONTENT_ENCODING] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[Headers.CONTENT_ENCODING] = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置Content-Encoding,表示内容编码
        /// </summary>
        public string ContentLanguage
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_LANGUAGE)
                    ? _metadata[Headers.CONTENT_LANGUAGE] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[Headers.CONTENT_LANGUAGE] = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置Cache-Control请求头
        /// </summary>
        public string CacheControl
        {
            get
            {
                return _metadata.ContainsKey(Headers.CACHE_CONTROL)
                    ? _metadata[Headers.CACHE_CONTROL] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[Headers.CACHE_CONTROL] = value;
                }
            }
        }

        /// <summary>
        /// 获取或者设置ETAG值
        /// </summary>
        public string ETag
        {
            get
            {
                return _metadata.ContainsKey(Headers.ETAG)
                    ? _metadata[Headers.ETAG] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[Headers.ETAG] = value;
                }
            }
        }

        /// <summary>
        /// 获取Last-Modified请求头的值，表示对象最后一次修改的时间
        /// </summary>
        public string LastModified
        {
            get
            {
                return _metadata.ContainsKey(Headers.LAST_MODIFIED)
                    ? _metadata[Headers.LAST_MODIFIED] as string : null;
            }
            internal set
            {
                _metadata[Headers.LAST_MODIFIED] = value;
            }
        }

        /// <summary>
        /// 获取Content-Disposition请求头，表示MIME用户代理如何显示附加的文件。
        /// </summary>
        public string ContentDisposition
        {
            get
            {
                return _metadata.ContainsKey(Headers.CONTENT_DISPOSITION)
                    ? _metadata[Headers.CONTENT_DISPOSITION] as string : null;
            }
            set
            {
                _metadata[Headers.CONTENT_DISPOSITION] = value;
            }
        }

        public ObjectMetadata()
        {
            ContentLength = -1L;
        }

        public void AddHeader(string key, object value)
        {
            _metadata.Add(key, value);
        }

        internal void Populate(IDictionary<string, string> requestHeaders)
        {
            foreach (var entry in _metadata)
                requestHeaders.Add(entry.Key, entry.Value.ToString());

            if (!requestHeaders.ContainsKey(Headers.CONTENT_TYPE))
                requestHeaders.Add(Headers.CONTENT_TYPE, FileContentType.MIMETYPE_OCTET_STREAM);

            foreach (var entry in _userMetadata)
                requestHeaders.Add(Headers.NOS_USER_METADATA_PREFIX + entry.Key, entry.Value);
        }
    }
}
