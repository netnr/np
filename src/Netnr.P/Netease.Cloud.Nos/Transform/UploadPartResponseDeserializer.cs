using System;
using System.IO;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Transform
{
    internal class UploadPartResponseDeserializer : ResponseDeserializer<UploadPartResult, UploadPartResult>
    {
        private readonly int _partNumber;

        public UploadPartResponseDeserializer(int partNumber)
            : base(null)
        {
            _partNumber = partNumber;
        }

        public override UploadPartResult Deserialize(ServiceReponse xmlStream)
        {
            var result = new UploadPartResult();
            if (xmlStream.Headers.ContainsKey(Headers.ETAG))
            {
                result.ETag = NosUtils.TrimQuotes(xmlStream.Headers[Headers.ETAG]);
            }
            result.PartNumber = _partNumber;

            return result;
        }
    }
}
