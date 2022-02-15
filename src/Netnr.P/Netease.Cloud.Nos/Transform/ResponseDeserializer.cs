using System;
using System.IO;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal abstract class ResponseDeserializer<TResult, TModel> : IDeserializer<ServiceReponse, TResult>
    {
        protected IDeserializer<Stream, TModel> ContentDeserializer { get; private set; }

        public ResponseDeserializer(IDeserializer<Stream, TModel> contentDeserializer)
        {
            ContentDeserializer = contentDeserializer;
        }

        public abstract TResult Deserialize(ServiceReponse xmlStream);

        protected string Decode(string value, string decodeType)
        {
            if (decodeType.Equals("url"))
                return HttpUtils.DecodeUri(value);

            return value;
        }
    }
}
