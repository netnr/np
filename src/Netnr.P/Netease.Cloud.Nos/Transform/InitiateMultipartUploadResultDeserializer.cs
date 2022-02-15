using System;
using System.IO;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;

namespace Netease.Cloud.NOS.Transform
{
    internal class InitiateMultipartUploadResultDeserializer
        : ResponseDeserializer<InitiateMultipartUploadResult, InitiateMultipartResponseModel>
    {
        public InitiateMultipartUploadResultDeserializer(IDeserializer<Stream, InitiateMultipartResponseModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override InitiateMultipartUploadResult Deserialize(ServiceReponse xmlStream)
        {
            var result = ContentDeserializer.Deserialize(xmlStream.Content);

            return new InitiateMultipartUploadResult
            {
                Bucket = result.Bucket,
                Key = result.Key,
                UploadId = result.UploadId
            };
        }
    }
}
