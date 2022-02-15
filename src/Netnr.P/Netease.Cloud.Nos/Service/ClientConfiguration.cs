using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS
{
    public class ClientConfiguration : ICloneable
    {
        //The default timeout for a connected socket.
        private static readonly int DEFAULT_SOCKET_TIMEOUT = 50 * 1000;

        // The default max connection pool size. 
        public static readonly int DEFAULT_MAX_CONNECTIONS = 50;

        //The default HTTP user agent header for Java SDK clients.
        public static readonly String DEFAULT_USER_AGENT = VersionInfoUtils.getUserAgent();

        //The default socket timeout.
        public static readonly int DEFAULT_CONNECTION_TIMEOUT = 50 * 1000;

        //The default maximum number of retries for error responses.
        public static readonly int DEFAULT_MAX_RETRIES = 3;

        public static readonly int ConnectionLimit = 512;

        private int _connectionTimeout = DEFAULT_CONNECTION_TIMEOUT;
        private int _maxConnections = DEFAULT_MAX_CONNECTIONS;
        private int _socketTimeout = DEFAULT_SOCKET_TIMEOUT;
        private int _maxErrorRetry = DEFAULT_MAX_RETRIES;
        private string _userAgent = DEFAULT_USER_AGENT;

        private int _proxyPort = -1;
        private Protocol _protocol = Protocol.Http;
        private string _caPath;

        private bool _isSubdomain = false;

        /// <summary>
        /// 获取或设置请求采用的通讯协议
        /// </summary>
        public Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        /// <summary>
        /// 获取或设置证书路径
        /// </summary>
        public string CaPath
        {
            get { return _caPath; }
            set { _caPath = value; }
        }

        /// <summary>
        /// 获取访问请求的User-Agent.
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// 获取或设置代理服务器的地址
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// 获取或设置代理服务器的端口
        /// </summary>
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// 获取或设置代理服务器授权用户所在的域
        /// </summary>
        public string ProxyDomain { get; set; }

        /// <summary>
        /// 设置或获取连接超时时间，默认：50000毫秒
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        /// <summary>
        /// 设置或获取允许打开的最大HTTP连接数 默认：50
        /// </summary>
        public int MaxConnections
        {
            get { return _maxConnections; }
            set { _maxConnections = value; }
        }

        /// <summary>
        /// 获取或设置Socket层传输数据超时时间（单位：毫秒）默认：50000毫秒
        /// </summary>
        public int SocketTimeout
        {
            get { return _socketTimeout; }
            set { _socketTimeout = value; }
        }

        /// <summary>
        /// 获取或设置请求失败后最大的重试次数 默认：3次
        /// </summary>
        public int MaxErrorRetry
        {
            get { return _maxErrorRetry; }
            set { _maxErrorRetry = value; }
        }

        /// <summary>
        /// 获取或设置请求的Endpoint是否是Subdomain
        /// </summary>
        public bool IsSubdomain
        {
            get
            { return _isSubdomain; }
            set
            { _isSubdomain = value; }
        }

        /// <summary>
        /// 获取自定义基准时间与本地时间的差值，单位秒
        /// </summary>
        public long TickOffset { get; internal set; }

        /// <summary>
        /// 获取该实例的拷贝。
        /// </summary>
        /// <returns>该实例的拷贝。</returns>
        public object Clone()
        {
            return new ClientConfiguration
            {
                ConnectionTimeout = ConnectionTimeout,
                MaxConnections = MaxConnections,
                MaxErrorRetry = MaxErrorRetry,
                SocketTimeout = SocketTimeout,
                ProxyDomain = ProxyDomain,
                ProxyHost = ProxyHost,
                ProxyPort = ProxyPort,
                ProxyUserName = ProxyUserName,
                ProxyPassword = ProxyPassword
            };
        }
    }
}
