using System;
using System.Collections.Generic;
using System.Diagnostics;

using Netease.Cloud.NOS.Util;
using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Authentication
{
    internal class NosSigner : ISigner
    {
        //表示用户想要访问的NOS资源路径
        private readonly string _resourcePath;

        /// <summary>
        /// 构造一个<see cref="NosSigner"/>的实例
        /// </summary>
        /// <param name="httpVerb">HTTP请求类型</param>
        /// <param name="resourcePath">用户想要访问的NOS资源路径</param>
        public NosSigner(string resourcePath)
        {
            this._resourcePath = resourcePath;

            if (resourcePath == null)
                throw new ArgumentNullException("Parameter resourcePath is empty", "resourcePath");
        }

        //生成NOS签名字符串，并包含在Head中
        public void Sign(ServiceRequest request, ICredentials credentials)
        {
            if (credentials == null) return;

            var accessKeyId = credentials.AccessKeyId;
            var accessKeySecret = credentials.AccessKeySecret;
            var httpMethod = request.Method.ToString().ToUpperInvariant();
            var resourcePath = _resourcePath;

            string canonicalString = RestUtils.makeNosCanonicalString(httpMethod, resourcePath, request, null);

            string signature = ServiceSignature.Create().ComputeSignature(accessKeySecret, canonicalString);

            request.Headers.Add("Authorization", "NOS " + accessKeyId + ":" + signature);
        }
    }
}
