using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Netease.Cloud.NOS
{
    public class NosObject : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// 获取或设置<see cref="NosObject"/>的<see cref="Key"/>
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 设置或获取<see cref="NosObject"/>所在的<see cref="Bucket"/>的名称
        /// </summary>
        public string Bucket { get; internal set; }

        /// <summary>
        /// 设置或获取<see cref="NosObject"/>的<see cref="ObjectMetadata"/>
        /// </summary>
        public ObjectMetadata objectMetadata { get; internal set; }

        /// <summary>
        /// 设置或获取<see cref="NosObject"/>的内容数据流
        /// </summary>
        public Stream Content { get; internal set; }

        /// <summary>
        /// 指定Key的<see cref="NosObject"/>构造函数
        /// </summary>
        /// <param name="key"></param>
        internal NosObject(string key)
        {
            Key = key;
        }

        /// <summary>
        /// <see cref="NosObject"/>构造函数
        /// </summary>
        internal NosObject()
        { }

        public override string ToString()
        {
            return string.Format(CultureInfo.InstalledUICulture, "[NosObject Key={0}, targetBucket={1}]", Key, Bucket ?? string.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (Content != null)
                    Content.Dispose();
                _disposed = true;
            }
        }
    }
}
