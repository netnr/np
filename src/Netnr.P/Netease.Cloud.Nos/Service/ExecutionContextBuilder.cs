using System;
using System.Collections.Generic;

using Netease.Cloud.NOS.Authentication;
using Netease.Cloud.NOS.Handlers;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Service
{
    internal class ExecutionContextBuilder
    {
        public IList<IResponseHandler> ResponseHandlers { get; private set; }

        public ICredentials Credentials { get; set; }

        public HttpMethod Method { get; set; }

        public string Bucket { get; set; }

        public string Key { get; set; }

        public ExecutionContextBuilder()
        {
            ResponseHandlers = new List<IResponseHandler>();
        }

        public ExecutionContext Build()
        {
            var executionContext = new ExecutionContext
            {
                Signer = CreateSigner(Bucket, Key),
                Credentials = Credentials
            };
            foreach (var h in ResponseHandlers)
            {
                executionContext.ResponseHandlers.Add(h);
            }

            return executionContext;
        }

        private static NosSigner CreateSigner(string bucket, string key)
        {
            var resourcePath = "/" + (bucket ?? string.Empty) +
                ((key != null ? "/" + NosUtils.UrlEncodeKey(key).Replace("/", "%2F").Replace("~", "%7E").Replace("%2A", "*") : ""));

            if (bucket != null && key == null)
            {
                resourcePath = resourcePath + "/";
            }
            return new NosSigner(resourcePath);
        }
    }
}
