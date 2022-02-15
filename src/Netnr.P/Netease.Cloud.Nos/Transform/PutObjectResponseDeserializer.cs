using System;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class PutObjectResponseDeserializer : ResponseDeserializer<PutObjectResult, PutObjectResult>
    {
        public PutObjectResponseDeserializer()
            : base(null)
        { }

        public override PutObjectResult Deserialize(ServiceReponse xmlStream)
        {
            var result = new PutObjectResult();
            if (xmlStream.Headers.ContainsKey(Headers.ETAG))
                result.ETag = NosUtils.TrimQuotes(xmlStream.Headers[Headers.ETAG]);
            return result;
        }
    }
}
