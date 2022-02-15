using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    internal static class HttpFactory
    {  
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;          
        }  

        private static void SetRequestHeaders(HttpWebRequest httpWebRequest, ServiceRequest serviceRequest, ClientConfiguration clientConfiguration)
        {
            httpWebRequest.Method = serviceRequest.Method.ToString().ToUpperInvariant();
            httpWebRequest.Timeout = clientConfiguration.ConnectionTimeout;

            foreach (var h in serviceRequest.Headers)
                HttpExtensions.AddInternalMethod(httpWebRequest.Headers, h.Key, h.Value);

            if (!string.IsNullOrEmpty(clientConfiguration.UserAgent))
                httpWebRequest.UserAgent = clientConfiguration.UserAgent;
        }

        private static void setRequestProxy(HttpWebRequest httpWebRequest, ClientConfiguration clientConfiguration)
        {
            httpWebRequest.Proxy = null;
            if (string.IsNullOrEmpty(clientConfiguration.ProxyHost))
            {
                if (clientConfiguration.ProxyPort < 0)
                    httpWebRequest.Proxy = new WebProxy(clientConfiguration.ProxyHost);
                else
                    httpWebRequest.Proxy = new WebProxy(clientConfiguration.ProxyHost, clientConfiguration.ProxyPort);
                if (!string.IsNullOrEmpty(clientConfiguration.ProxyUserName))
                {
                    httpWebRequest.Proxy.Credentials = string.IsNullOrEmpty(clientConfiguration.ProxyDomain) ?
                        new NetworkCredential(clientConfiguration.ProxyUserName, clientConfiguration.ProxyPassword ?? string.Empty) :
                        new NetworkCredential(clientConfiguration.ProxyUserName, clientConfiguration.ProxyPassword ?? string.Empty, clientConfiguration.ProxyDomain);
                }
            }
        }

        internal static HttpWebRequest CreateWebRequest(ServiceRequest serviceRequest, ClientConfiguration clientConfiguration)
        {
            var request = WebRequest.Create(serviceRequest.CreateRequestUri()) as HttpWebRequest;

            SetRequestHeaders(request, serviceRequest, clientConfiguration);
            setRequestProxy(request, clientConfiguration);

            if (request.RequestUri.Scheme == "https")
            {                
                #region 加载证书
                //挂接验证服务端证书的回调                
                X509Certificate cer = new X509Certificate(clientConfiguration.CaPath);
                request.ClientCertificates.Add(cer);
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                #endregion

            }

            return request;
        }
    }

    internal static class HttpExtensions
    {
        //private static MethodInfo _addInternalMethod;
        private static bool? _isMonoPlatform;

        private static readonly ICollection<PlatformID> MonoPlatforms = new List<PlatformID> { PlatformID.MacOSX, PlatformID.Unix };

        internal static void AddInternalMethod(WebHeaderCollection headers, string key, string value)
        {
            if (_isMonoPlatform == null)
                _isMonoPlatform = MonoPlatforms.Contains(Environment.OSVersion.Platform);

            if (_isMonoPlatform == false)
                value = HttpUtils.Reencode(value, "utf-8", "iso-8859-1");

            //if (_addInternalMethod == null)
            //{
            //    var methodName = (_isMonoPlatform == true) ? "AddWithoutValidate" : "AddInternal";
            //    var mi = typeof(WebHeaderCollection).GetMethod(
            //        methodName,
            //        BindingFlags.NonPublic | BindingFlags.Instance,
            //        null,
            //        new Type[] { typeof(string), typeof(string) },
            //        null
            //        );
            //    _addInternalMethod = mi;
            //}
            //_addInternalMethod?.Invoke(headers, new object[] { key, value });
            
            headers.Add(key, value);
        }
    }
}
